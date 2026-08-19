using Sales.Application.DTOs.Orders;

namespace Sales.Application.Interfaces;

public interface IOrderService
{
    Task<OrderResponse> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<OrderResponse>> GetAllAsync(
        int? clientId = null,
        DateTime? dateFrom = null,
        DateTime? dateTo = null,
        CancellationToken cancellationToken = default);

    Task<OrderResponse> CreateAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<OrderResponse> AddItemAsync(
        int orderId,
        AddOrderItemRequest request,
        CancellationToken cancellationToken = default);

    Task<OrderResponse> UpdateItemAsync(
        int orderId,
        int productId,
        UpdateOrderItemRequest request,
        CancellationToken cancellationToken = default);

    Task<OrderResponse> RemoveItemAsync(
        int orderId,
        int productId,
        CancellationToken cancellationToken = default);
}