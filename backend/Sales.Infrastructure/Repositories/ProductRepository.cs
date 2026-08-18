using Dapper;
using Sales.Application.Interfaces;
using Sales.Domain.Entities;
using Sales.Infrastructure.Database;

namespace Sales.Infrastructure.Repositories;

public sealed class ProductRepository : IProductRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public ProductRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Product?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT
                CodProduto AS Id,
                Nome AS Name,
                Preco AS Price,
                Estoque AS Stock
            FROM Produto
            WHERE CodProduto = @Id;
            """;

        using var connection = _connectionFactory.CreateConnection();

        return await connection.QuerySingleOrDefaultAsync<Product>(
            new CommandDefinition(
                sql,
                new { Id = id },
                cancellationToken: cancellationToken));
    }

    public async Task<IReadOnlyCollection<Product>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT
                CodProduto AS Id,
                Nome AS Name,
                Preco AS Price,
                Estoque AS Stock
            FROM Produto
            ORDER BY Nome;
            """;

        using var connection = _connectionFactory.CreateConnection();

        var products = await connection.QueryAsync<Product>(
            new CommandDefinition(
                sql,
                cancellationToken: cancellationToken));

        return products.ToList();
    }

    public async Task<int> CreateAsync(
        Product product,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO Produto
            (
                Nome,
                Preco,
                Estoque
            )
            VALUES
            (
                @Name,
                @Price,
                @Stock
            );

            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """;

        using var connection = _connectionFactory.CreateConnection();

        return await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                sql,
                product,
                cancellationToken: cancellationToken));
    }

    public async Task UpdateAsync(
        Product product,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            UPDATE Produto
            SET
                Nome = @Name,
                Preco = @Price,
                Estoque = @Stock
            WHERE CodProduto = @Id;
            """;

        using var connection = _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                product,
                cancellationToken: cancellationToken));
    }
}