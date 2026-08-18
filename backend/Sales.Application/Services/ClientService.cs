using Sales.Application.DTOs.Clients;
using Sales.Application.Exceptions;
using Sales.Application.Interfaces;
using Sales.Domain.Entities;

namespace Sales.Application.Services;

public sealed class ClientService : IClientService
{
    private readonly IClientRepository _clientRepository;

    public ClientService(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }

    public async Task<ClientResponse?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var client = await _clientRepository.GetByIdAsync(
            id,
            cancellationToken);

        return client is null
            ? null
            : MapToResponse(client);
    }

    public async Task<IReadOnlyCollection<ClientResponse>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var clients = await _clientRepository.GetAllAsync(
            cancellationToken);

        return clients
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<ClientResponse> CreateAsync(
        CreateClientRequest request,
        CancellationToken cancellationToken = default)
    {
        Validate(request);

        var normalizedCnpj = NormalizeCnpj(request.Cnpj);

        var existingClient = await _clientRepository.GetByCnpjAsync(
            normalizedCnpj,
            cancellationToken);

        if (existingClient is not null)
        {
            throw new ConflictException(
                "Já existe um cliente cadastrado com este CNPJ.");
        }

        var client = new Client(
            normalizedCnpj,
            request.Name.Trim(),
            request.Email.Trim());

        var id = await _clientRepository.CreateAsync(
            client,
            cancellationToken);

        var createdClient = await _clientRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (createdClient is null)
        {
            throw new InvalidOperationException(
                "O cliente foi criado, mas não pôde ser recuperado.");
        }

        return MapToResponse(createdClient);
    }

    private static void Validate(CreateClientRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Cnpj))
        {
            throw new ArgumentException(
                "O CNPJ é obrigatório.");
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException(
                "O nome do cliente é obrigatório.");
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            throw new ArgumentException(
                "O e-mail do cliente é obrigatório.");
        }
    }

    private static string NormalizeCnpj(string cnpj)
    {
        return new string(
            cnpj.Where(char.IsDigit).ToArray());
    }

    private static ClientResponse MapToResponse(Client client)
    {
        return new ClientResponse(
            client.Id,
            client.Cnpj,
            client.Name,
            client.Email,
            client.CreatedAt);
    }
}