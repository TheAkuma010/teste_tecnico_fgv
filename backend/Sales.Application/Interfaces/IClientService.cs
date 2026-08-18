using Sales.Application.DTOs.Clients;

namespace Sales.Application.Interfaces;

public interface IClientService
{
    Task<ClientResponse?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<ClientResponse>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<ClientResponse> CreateAsync(
        CreateClientRequest request,
        CancellationToken cancellationToken = default);
}