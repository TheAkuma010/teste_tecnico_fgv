using Sales.Domain.Entities;

namespace Sales.Application.Interfaces;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<OrderItem?> GetItemAsync(
        int orderId,
        int productId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Order>> GetAllAsync(
        int? clientId = null,
        DateTime? dateFrom = null,
        DateTime? dateTo = null,
        CancellationToken cancellationToken = default);

    Task<int> CreateAsync(
        int clientId,
        DateTime createdAt,
        CancellationToken cancellationToken = default);

    Task AddItemAsync(
        int orderId,
        int productId,
        int quantity,
        decimal unitPrice,
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

    Task UpdateTotalAsync(
        int orderId,
        decimal total,
        CancellationToken cancellationToken = default);
}
