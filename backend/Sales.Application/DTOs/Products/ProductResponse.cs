namespace Sales.Application.DTOs.Products;

public sealed record ProductResponse(
    int Id,
    string Name,
    decimal Price,
    int Stock);