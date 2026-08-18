namespace Sales.Application.DTOs.Products;

public sealed record UpdateProductRequest(
    string Name,
    decimal Price,
    int Stock);