namespace Sales.Application.DTOs.Clients;

public sealed record CreateClientRequest(
    string Cnpj,
    string Name,
    string Email);