namespace Sales.Application.DTOs.Orders;

public sealed record OrderResponse(
    int Id,
    int ClientId,
    string ClientName,
    DateTime CreatedAt,
    decimal Total,
    IReadOnlyCollection<OrderItemResponse> Items);