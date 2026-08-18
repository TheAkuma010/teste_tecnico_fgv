using Dapper;
using Sales.Application.Interfaces;
using Sales.Domain.Entities;
using Sales.Infrastructure.Database;

namespace Sales.Infrastructure.Repositories;

public sealed class ClientRepository : IClientRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public ClientRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Client?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT
                CodCliente AS Id,
                CNPJ AS Cnpj,
                Nome AS Name,
                Email,
                DataCadastro AS CreatedAt
            FROM Cliente
            WHERE CodCliente = @Id;
            """;

        using var connection = _connectionFactory.CreateConnection();

        return await connection.QuerySingleOrDefaultAsync<Client>(
            new CommandDefinition(
                sql,
                new { Id = id },
                cancellationToken: cancellationToken));
    }

    public async Task<Client?> GetByCnpjAsync(
        string cnpj,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT
                CodCliente AS Id,
                CNPJ AS Cnpj,
                Nome AS Name,
                Email,
                DataCadastro AS CreatedAt
            FROM Cliente
            WHERE CNPJ = @Cnpj;
            """;

        using var connection = _connectionFactory.CreateConnection();

        return await connection.QuerySingleOrDefaultAsync<Client>(
            new CommandDefinition(
                sql,
                new { Cnpj = cnpj },
                cancellationToken: cancellationToken));
    }

    public async Task<IReadOnlyCollection<Client>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT
                CodCliente AS Id,
                CNPJ AS Cnpj,
                Nome AS Name,
                Email,
                DataCadastro AS CreatedAt
            FROM Cliente
            ORDER BY Nome;
            """;

        using var connection = _connectionFactory.CreateConnection();

        var clients = await connection.QueryAsync<Client>(
            new CommandDefinition(
                sql,
                cancellationToken: cancellationToken));

        return clients.ToList();
    }

    public async Task<int> CreateAsync(
        Client client,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO Cliente
            (
                CNPJ,
                Nome,
                Email,
                DataCadastro
            )
            VALUES
            (
                @Cnpj,
                @Name,
                @Email,
                @CreatedAt
            );

            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """;

        using var connection = _connectionFactory.CreateConnection();

        return await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                sql,
                client,
                cancellationToken: cancellationToken));
    }
}