using Sales.Domain.Entities;

namespace Sales.Application.Interfaces;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Order>> GetAllAsync(
        int? clientId = null,
        DateTime? dateFrom = null,
        DateTime? dateTo = null,
        CancellationToken cancellationToken = default);

    Task<int> CreateAsync(
        Order order,
        CancellationToken cancellationToken = default);

    Task AddItemAsync(
        int orderId,
        OrderItem item,
        CancellationToken cancellationToken = default);

    Task UpdateItemAsync(
        int orderId,
        int productId,
        int quantity,
        decimal unitPrice,
        CancellationToken cancellationToken = default);

    Task RemoveItemAsync(
        int orderId,
        int productId,
        CancellationToken cancellationToken = default);
}