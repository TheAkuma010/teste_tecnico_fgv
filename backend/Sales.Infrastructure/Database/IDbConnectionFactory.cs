using System.Data;

namespace Sales.Infrastructure.Database;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}