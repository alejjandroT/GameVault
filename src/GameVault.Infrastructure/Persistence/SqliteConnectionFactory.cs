using System.Data;
using Microsoft.Data.Sqlite;

namespace GameVault.Infrastructure.Persistence;

public interface ISqliteConnectionFactory
{
    IDbConnection CreateConnection();
}

public sealed class SqliteConnectionFactory : ISqliteConnectionFactory
{
    private readonly string _connectionString;

    public SqliteConnectionFactory(string connectionString)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);
        _connectionString = connectionString;
    }

    public IDbConnection CreateConnection() => new SqliteConnection(_connectionString);
}
