using Sales.Application.DTOs.Products;
using Sales.Application.Exceptions;
using Sales.Application.Interfaces;
using Sales.Domain.Entities;

namespace Sales.Application.Services;

public sealed class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductResponse?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(
            id,
            cancellationToken);

        return product is null
            ? null
            : MapToResponse(product);
    }

    public async Task<IReadOnlyCollection<ProductResponse>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var products = await _productRepository.GetAllAsync(
            cancellationToken);

        return products
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<ProductResponse> CreateAsync(
        CreateProductRequest request,
        CancellationToken cancellationToken = default)
    {
        Validate(
            request.Name,
            request.Price,
            request.Stock);

        var product = new Product(
            request.Name.Trim(),
            request.Price,
            request.Stock);

        var id = await _productRepository.CreateAsync(
            product,
            cancellationToken);

        var createdProduct = await _productRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (createdProduct is null)
        {
            throw new InvalidOperationException(
                "O produto foi criado, mas não pôde ser recuperado.");
        }

        return MapToResponse(createdProduct);
    }

    public async Task<ProductResponse> UpdateAsync(
        int id,
        UpdateProductRequest request,
        CancellationToken cancellationToken = default)
    {
        Validate(
            request.Name,
            request.Price,
            request.Stock);

        var product = await _productRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (product is null)
        {
            throw new NotFoundException("Produto", id);
        }

        product.Update(
            request.Name.Trim(),
            request.Price,
            request.Stock);

        await _productRepository.UpdateAsync(
            product,
            cancellationToken);

        return MapToResponse(product);
    }

    private static void Validate(
        string name,
        decimal price,
        int stock)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "O nome do produto é obrigatório.");
        }

        if (price < 0)
        {
            throw new ArgumentException(
                "O preço do produto não pode ser negativo.");
        }

        if (stock < 0)
        {
            throw new ArgumentException(
                "O estoque não pode ser negativo.");
        }
    }

    private static ProductResponse MapToResponse(Product product)
    {
        return new ProductResponse(
            product.Id,
            product.Name,
            product.Price,
            product.Stock);
    }
}