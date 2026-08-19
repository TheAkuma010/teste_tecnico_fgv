using Sales.Application.DTOs.Orders;
using Sales.Application.Exceptions;
using Sales.Application.Interfaces;
using Sales.Domain.Entities;

namespace Sales.Application.Services;

public sealed class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;

    public OrderService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<OrderResponse> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(
            id,
            cancellationToken);

        if (order is null)
        {
            throw new NotFoundException("Pedido", id);
        }

        return MapToResponse(order);
    }

    public async Task<IReadOnlyCollection<OrderResponse>> GetAllAsync(
        int? clientId = null,
        DateTime? dateFrom = null,
        DateTime? dateTo = null,
        CancellationToken cancellationToken = default)
    {
        var orders = await _unitOfWork.Orders.GetAllAsync(
            clientId,
            dateFrom,
            dateTo,
            cancellationToken);

        return orders
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<OrderResponse> CreateAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.ClientCnpj))
        {
            throw new ArgumentException(
                "O CNPJ do cliente é obrigatório.");
        }

        var client = await _unitOfWork.Clients.GetByCnpjAsync(
            NormalizeCnpj(request.ClientCnpj),
            cancellationToken);

        if (client is null)
        {
            throw new NotFoundException("Cliente", request.ClientCnpj);
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var orderId = await _unitOfWork.Orders.CreateAsync(
                client.Id,
                DateTime.UtcNow,
                cancellationToken);

            await _unitOfWork.CommitAsync(cancellationToken);

            return await GetByIdAsync(
                orderId,
                cancellationToken);
        }
        catch
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<OrderResponse> AddItemAsync(
        int orderId,
        AddOrderItemRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidateQuantity(request.Quantity);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            await EnsureOrderExistsAsync(
                orderId,
                cancellationToken);

            var product = await GetRequiredProductAsync(
                request.ProductId,
                cancellationToken);

            product.DecreaseStock(request.Quantity);

            var existingItem = await _unitOfWork.Orders.GetItemAsync(
                orderId,
                request.ProductId,
                cancellationToken);

            if (existingItem is null)
            {
                await _unitOfWork.Orders.AddItemAsync(
                    orderId,
                    product.Id,
                    request.Quantity,
                    product.Price,
                    cancellationToken);
            }
            else
            {
                await _unitOfWork.Orders.UpdateItemAsync(
                    orderId,
                    product.Id,
                    existingItem.Quantity + request.Quantity,
                    product.Price,
                    cancellationToken);
            }

            await _unitOfWork.Products.UpdateAsync(
                product,
                cancellationToken);

            var order = await RefreshTotalAsync(
                orderId,
                cancellationToken);

            await _unitOfWork.CommitAsync(cancellationToken);

            return MapToResponse(order);
        }
        catch
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<OrderResponse> UpdateItemAsync(
        int orderId,
        int productId,
        UpdateOrderItemRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidateQuantity(request.Quantity);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            await EnsureOrderExistsAsync(
                orderId,
                cancellationToken);

            var existingItem = await GetRequiredItemAsync(
                orderId,
                productId,
                cancellationToken);

            var product = await GetRequiredProductAsync(
                productId,
                cancellationToken);

            var quantityDelta = request.Quantity - existingItem.Quantity;

            if (quantityDelta > 0)
            {
                product.DecreaseStock(quantityDelta);
            }
            else if (quantityDelta < 0)
            {
                product.IncreaseStock(Math.Abs(quantityDelta));
            }

            await _unitOfWork.Orders.UpdateItemAsync(
                orderId,
                productId,
                request.Quantity,
                product.Price,
                cancellationToken);

            await _unitOfWork.Products.UpdateAsync(
                product,
                cancellationToken);

            var order = await RefreshTotalAsync(
                orderId,
                cancellationToken);

            await _unitOfWork.CommitAsync(cancellationToken);

            return MapToResponse(order);
        }
        catch
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<OrderResponse> RemoveItemAsync(
        int orderId,
        int productId,
        CancellationToken cancellationToken = default)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            await EnsureOrderExistsAsync(
                orderId,
                cancellationToken);

            var existingItem = await GetRequiredItemAsync(
                orderId,
                productId,
                cancellationToken);

            var product = await GetRequiredProductAsync(
                productId,
                cancellationToken);

            product.IncreaseStock(existingItem.Quantity);

            await _unitOfWork.Orders.RemoveItemAsync(
                orderId,
                productId,
                cancellationToken);

            await _unitOfWork.Products.UpdateAsync(
                product,
                cancellationToken);

            var order = await RefreshTotalAsync(
                orderId,
                cancellationToken);

            await _unitOfWork.CommitAsync(cancellationToken);

            return MapToResponse(order);
        }
        catch
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private async Task<Order> RefreshTotalAsync(
        int orderId,
        CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(
            orderId,
            cancellationToken);

        if (order is null)
        {
            throw new NotFoundException("Pedido", orderId);
        }

        var total = order.OrderItems.Sum(item => item.Total);
        order.SetTotal(total);

        await _unitOfWork.Orders.UpdateTotalAsync(
            orderId,
            total,
            cancellationToken);

        return order;
    }

    private async Task EnsureOrderExistsAsync(
        int orderId,
        CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(
            orderId,
            cancellationToken);

        if (order is null)
        {
            throw new NotFoundException("Pedido", orderId);
        }
    }

    private async Task<Product> GetRequiredProductAsync(
        int productId,
        CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(
            productId,
            cancellationToken);

        if (product is null)
        {
            throw new NotFoundException("Produto", productId);
        }

        return product;
    }

    private async Task<OrderItem> GetRequiredItemAsync(
        int orderId,
        int productId,
        CancellationToken cancellationToken)
    {
        var item = await _unitOfWork.Orders.GetItemAsync(
            orderId,
            productId,
            cancellationToken);

        if (item is null)
        {
            throw new NotFoundException("Item do pedido", productId);
        }

        return item;
    }

    private static void ValidateQuantity(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentException(
                "A quantidade deve ser maior que zero.");
        }
    }

    private static string NormalizeCnpj(string cnpj)
    {
        return new string(
            cnpj.Where(char.IsDigit).ToArray());
    }

    private static OrderResponse MapToResponse(Order order)
    {
        return new OrderResponse(
            order.Id,
            order.ClientId,
            order.ClientName,
            order.CreatedAt,
            order.Total,
            order.OrderItems
                .Select(item => new OrderItemResponse(
                    item.ProductId,
                    item.ProductName,
                    item.Quantity,
                    item.UnitPrice,
                    item.Total))
                .ToList());
    }
}
