using Dapper;
using Sales.Application.Interfaces;
using Sales.Domain.Entities;
using Sales.Infrastructure.Database;

namespace Sales.Infrastructure.Repositories;

public sealed class ProductRepository : IProductRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly DbTransactionContext _transactionContext;

    public ProductRepository(IDbConnectionFactory connectionFactory, DbTransactionContext transactionContext)
    {
        _connectionFactory = connectionFactory;
        _transactionContext = transactionContext;
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

        var (connection, transaction, shouldDispose) = GetConnection();

        try
        {
            return await connection.QuerySingleOrDefaultAsync<Product>(
                new CommandDefinition(
                    sql,
                    new { Id = id },
                    transaction,
                    cancellationToken: cancellationToken));
        }
        finally
        {
            if (shouldDispose)
            {
                connection.Dispose();
            }
        }
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

        var (connection, transaction, shouldDispose) = GetConnection();

        try
        {
            var products = await connection.QueryAsync<Product>(
                new CommandDefinition(
                    sql,
                    transaction: transaction,
                    cancellationToken: cancellationToken));

            return products.ToList();
        }
        finally
        {
            if (shouldDispose)
            {
                connection.Dispose();
            }
        }
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

        var (connection, transaction, shouldDispose) = GetConnection();

        try
        {
            return await connection.ExecuteScalarAsync<int>(
                new CommandDefinition(
                    sql,
                    product,
                    transaction,
                    cancellationToken: cancellationToken));
        }
        finally
        {
            if (shouldDispose)
            {
                connection.Dispose();
            }
        }
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

        var (connection, transaction, shouldDispose) = GetConnection();

        try
        {
            await connection.ExecuteAsync(
                new CommandDefinition(
                    sql,
                    product,
                    transaction,
                    cancellationToken: cancellationToken));
        }
        finally
        {
            if (shouldDispose)
            {
                connection.Dispose();
            }
        }
    }

    private (System.Data.IDbConnection Connection, System.Data.IDbTransaction? Transaction, bool ShouldDispose) GetConnection()
    {
        if (_transactionContext.IsActive)
        {
            return (
                _transactionContext.Connection,
                _transactionContext.Transaction,
                false);
        }

        return (
            _connectionFactory.CreateConnection(),
            null,
            true);
    }
}
