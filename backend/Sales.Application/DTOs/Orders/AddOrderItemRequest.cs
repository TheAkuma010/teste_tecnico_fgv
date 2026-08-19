namespace Sales.Application.DTOs.Orders;

public sealed record AddOrderItemRequest(
    int ProductId,
    int Quantity);