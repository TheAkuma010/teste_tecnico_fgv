using System.Data;

namespace Sales.Infrastructure.Database;

public interface IDbTransactionContext
{
    IDbConnection Connection { get; }

    IDbTransaction Transaction { get; }

    bool IsActive { get; }
}