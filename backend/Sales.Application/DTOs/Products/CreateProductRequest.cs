namespace Sales.Application.DTOs.Products;

public sealed record CreateProductRequest(
    string Name,
    decimal Price,
    int Stock);