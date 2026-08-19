using System.Data;
using Microsoft.Data.SqlClient;
using Sales.Application.Interfaces;

namespace Sales.Infrastructure.Database;

public sealed class SqlUnitOfWork : IUnitOfWork
{
    private readonly IDbConnectionFactory _connectionFactory;

    private IDbConnection? _connection;
    private IDbTransaction? _transaction;

    public IClientRepository Clients { get; }
    public IProductRepository Products { get; }
    public IOrderRepository Orders { get; }

    public SqlUnitOfWork(
        IDbConnectionFactory connectionFactory,
        IClientRepository clients,
        IProductRepository products,
        IOrderRepository orders)
    {
        _connectionFactory = connectionFactory;
        Clients = clients;
        Products = products;
        Orders = orders;
    }

    public async Task BeginTransactionAsync(
        CancellationToken cancellationToken = default)
    {
        _connection = _connectionFactory.CreateConnection();

        if (_connection is SqlConnection sqlConnection)
        {
            await sqlConnection.OpenAsync(cancellationToken);
        }
        else
        {
            _connection.Open();
        }

        _transaction = _connection.BeginTransaction();
    }

    public Task CommitAsync(
        CancellationToken cancellationToken = default)
    {
        _transaction?.Commit();

        return Task.CompletedTask;
    }

    public Task RollbackAsync(
        CancellationToken cancellationToken = default)
    {
        _transaction?.Rollback();

        return Task.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        _transaction?.Dispose();
        _connection?.Dispose();

        return ValueTask.CompletedTask;
    }
}