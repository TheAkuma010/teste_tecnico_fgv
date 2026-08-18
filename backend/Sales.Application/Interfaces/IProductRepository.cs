using Sales.Domain.Entities;

namespace Sales.Application.Interfaces;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Product>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<int> CreateAsync(
        Product product,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        Product product,
        CancellationToken cancellationToken = default);
}