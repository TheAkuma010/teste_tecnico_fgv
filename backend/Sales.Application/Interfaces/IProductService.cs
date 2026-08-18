using Sales.Application.DTOs.Products;

namespace Sales.Application.Interfaces;

public interface IProductService
{
    Task<ProductResponse?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<ProductResponse>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<ProductResponse> CreateAsync(
        CreateProductRequest request,
        CancellationToken cancellationToken = default);

    Task<ProductResponse> UpdateAsync(
        int id,
        UpdateProductRequest request,
        CancellationToken cancellationToken = default);
}