namespace Sales.Application.DTOs.Clients;

public sealed record ClientResponse(
    int Id,
    string Cnpj,
    string Name,
    string Email,
    DateTime CreatedAt);