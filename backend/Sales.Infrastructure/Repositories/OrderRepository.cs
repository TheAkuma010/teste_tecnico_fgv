using System.Data;
using Dapper;
using Sales.Application.Interfaces;
using Sales.Domain.Entities;
using Sales.Infrastructure.Database;

namespace Sales.Infrastructure.Repositories;

public sealed class OrderRepository : IOrderRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly DbTransactionContext _transactionContext;

    public OrderRepository(
        IDbConnectionFactory connectionFactory,
        DbTransactionContext transactionContext)
    {
        _connectionFactory = connectionFactory;
        _transactionContext = transactionContext;
    }

    public async Task<Order?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT
                p.CodPedido AS Id,
                p.CodCliente AS ClientId,
                c.Nome AS ClientName,
                p.DataPedido AS CreatedAt,
                p.ValorTotal AS Total,
                ip.CodPedido AS OrderId,
                ip.CodProduto AS ProductId,
                pr.Nome AS ProductName,
                ip.Quantidade AS Quantity,
                ip.PrecoUnitario AS UnitPrice
            FROM Pedido p
            INNER JOIN Cliente c ON c.CodCliente = p.CodCliente
            LEFT JOIN ItensPedido ip ON ip.CodPedido = p.CodPedido
            LEFT JOIN Produto pr ON pr.CodProduto = ip.CodProduto
            WHERE p.CodPedido = @Id
            ORDER BY pr.Nome;
            """;

        return await QuerySingleOrderAsync(
            sql,
            new { Id = id },
            cancellationToken);
    }

    public async Task<OrderItem?> GetItemAsync(
        int orderId,
        int productId,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT
                ip.CodPedido AS OrderId,
                ip.CodProduto AS ProductId,
                pr.Nome AS ProductName,
                ip.Quantidade AS Quantity,
                ip.PrecoUnitario AS UnitPrice
            FROM ItensPedido ip
            INNER JOIN Produto pr ON pr.CodProduto = ip.CodProduto
            WHERE ip.CodPedido = @OrderId
              AND ip.CodProduto = @ProductId;
            """;

        return await QuerySingleOrDefaultAsync<OrderItem>(
            sql,
            new { OrderId = orderId, ProductId = productId },
            cancellationToken);
    }

    public async Task<IReadOnlyCollection<Order>> GetAllAsync(
        int? clientId = null,
        DateTime? dateFrom = null,
        DateTime? dateTo = null,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT
                p.CodPedido AS Id,
                p.CodCliente AS ClientId,
                c.Nome AS ClientName,
                p.DataPedido AS CreatedAt,
                p.ValorTotal AS Total,
                ip.CodPedido AS OrderId,
                ip.CodProduto AS ProductId,
                pr.Nome AS ProductName,
                ip.Quantidade AS Quantity,
                ip.PrecoUnitario AS UnitPrice
            FROM Pedido p
            INNER JOIN Cliente c ON c.CodCliente = p.CodCliente
            LEFT JOIN ItensPedido ip ON ip.CodPedido = p.CodPedido
            LEFT JOIN Produto pr ON pr.CodProduto = ip.CodProduto
            WHERE (@ClientId IS NULL OR p.CodCliente = @ClientId)
              AND (@DateFrom IS NULL OR p.DataPedido >= @DateFrom)
              AND (@DateTo IS NULL OR p.DataPedido < @DateTo)
            ORDER BY p.DataPedido DESC, p.CodPedido DESC, pr.Nome;
            """;

        return await QueryOrdersAsync(
            sql,
            new
            {
                ClientId = clientId,
                DateFrom = dateFrom,
                DateTo = dateTo?.Date.AddDays(1)
            },
            cancellationToken);
    }

    public async Task<int> CreateAsync(
        int clientId,
        DateTime createdAt,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO Pedido
            (
                CodCliente,
                DataPedido,
                ValorTotal
            )
            VALUES
            (
                @ClientId,
                @CreatedAt,
                0
            );

            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """;

        return await ExecuteScalarAsync<int>(
            sql,
            new { ClientId = clientId, CreatedAt = createdAt },
            cancellationToken);
    }

    public async Task AddItemAsync(
        int orderId,
        int productId,
        int quantity,
        decimal unitPrice,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO ItensPedido
            (
                CodPedido,
                CodProduto,
                Quantidade,
                PrecoUnitario
            )
            VALUES
            (
                @OrderId,
                @ProductId,
                @Quantity,
                @UnitPrice
            );
            """;

        await ExecuteAsync(
            sql,
            new { OrderId = orderId, ProductId = productId, Quantity = quantity, UnitPrice = unitPrice },
            cancellationToken);
    }

    public async Task UpdateItemAsync(
        int orderId,
        int productId,
        int quantity,
        decimal unitPrice,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            UPDATE ItensPedido
            SET
                Quantidade = @Quantity,
                PrecoUnitario = @UnitPrice
            WHERE CodPedido = @OrderId
              AND CodProduto = @ProductId;
            """;

        await ExecuteAsync(
            sql,
            new { OrderId = orderId, ProductId = productId, Quantity = quantity, UnitPrice = unitPrice },
            cancellationToken);
    }

    public async Task RemoveItemAsync(
        int orderId,
        int productId,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            DELETE FROM ItensPedido
            WHERE CodPedido = @OrderId
              AND CodProduto = @ProductId;
            """;

        await ExecuteAsync(
            sql,
            new { OrderId = orderId, ProductId = productId },
            cancellationToken);
    }

    public async Task UpdateTotalAsync(
        int orderId,
        decimal total,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            UPDATE Pedido
            SET ValorTotal = @Total
            WHERE CodPedido = @OrderId;
            """;

        await ExecuteAsync(
            sql,
            new { OrderId = orderId, Total = total },
            cancellationToken);
    }

    private async Task<Order?> QuerySingleOrderAsync(
        string sql,
        object parameters,
        CancellationToken cancellationToken)
    {
        var orders = await QueryOrdersAsync(
            sql,
            parameters,
            cancellationToken);

        return orders.SingleOrDefault();
    }

    private async Task<IReadOnlyCollection<Order>> QueryOrdersAsync(
        string sql,
        object parameters,
        CancellationToken cancellationToken)
    {
        var orders = new Dictionary<int, Order>();
        var (connection, transaction, shouldDispose) = GetConnection();

        try
        {
            await connection.QueryAsync<Order, OrderItem, Order>(
                new CommandDefinition(
                    sql,
                    parameters,
                    transaction,
                    cancellationToken: cancellationToken),
                (order, item) =>
                {
                    if (!orders.TryGetValue(order.Id, out var existingOrder))
                    {
                        existingOrder = order;
                        orders.Add(order.Id, existingOrder);
                    }

                    if (item.ProductId > 0)
                    {
                        existingOrder.AddItem(item);
                    }

                    return existingOrder;
                },
                splitOn: "OrderId");

            return orders.Values.ToList();
        }
        finally
        {
            if (shouldDispose)
            {
                connection.Dispose();
            }
        }
    }

    private async Task<T?> QuerySingleOrDefaultAsync<T>(
        string sql,
        object parameters,
        CancellationToken cancellationToken)
    {
        var (connection, transaction, shouldDispose) = GetConnection();

        try
        {
            return await connection.QuerySingleOrDefaultAsync<T>(
                new CommandDefinition(
                    sql,
                    parameters,
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

    private async Task<T> ExecuteScalarAsync<T>(
        string sql,
        object parameters,
        CancellationToken cancellationToken)
    {
        var (connection, transaction, shouldDispose) = GetConnection();

        try
        {
            return await connection.ExecuteScalarAsync<T>(
                new CommandDefinition(
                    sql,
                    parameters,
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

    private async Task ExecuteAsync(
        string sql,
        object parameters,
        CancellationToken cancellationToken)
    {
        var (connection, transaction, shouldDispose) = GetConnection();

        try
        {
            await connection.ExecuteAsync(
                new CommandDefinition(
                    sql,
                    parameters,
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

    private (IDbConnection Connection, IDbTransaction? Transaction, bool ShouldDispose) GetConnection()
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
