using System.Data;

namespace Sales.Infrastructure.Database;

public sealed class DbTransactionContext : IDbTransactionContext
{
    private IDbConnection? _connection;
    private IDbTransaction? _transaction;

    public IDbConnection Connection =>
        _connection
        ?? throw new InvalidOperationException(
            "Nenhuma conexão ativa foi inicializada.");

    public IDbTransaction Transaction =>
        _transaction
        ?? throw new InvalidOperationException(
            "Nenhuma transação ativa foi inicializada.");

    public bool IsActive =>
        _connection is not null &&
        _transaction is not null;

    public void Start(
        IDbConnection connection,
        IDbTransaction transaction)
    {
        if (IsActive)
        {
            throw new InvalidOperationException(
                "Já existe uma transação ativa.");
        }

        _connection = connection;
        _transaction = transaction;
    }

    public void Clear()
    {
        _transaction?.Dispose();
        _connection?.Dispose();

        _transaction = null;
        _connection = null;
    }
}