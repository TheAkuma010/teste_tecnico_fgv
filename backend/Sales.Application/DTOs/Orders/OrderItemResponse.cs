namespace Sales.Application.DTOs.Orders;

public sealed record OrderItemResponse(
    int ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal Total);