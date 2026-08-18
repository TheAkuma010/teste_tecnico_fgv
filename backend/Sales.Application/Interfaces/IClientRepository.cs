using Sales.Domain.Entities;

namespace Sales.Application.Interfaces;

public interface IClientRepository
{
    Task<Client?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<Client?> GetByCnpjAsync(
        string cnpj,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Client>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<int> CreateAsync(
        Client client,
        CancellationToken cancellationToken = default);
}