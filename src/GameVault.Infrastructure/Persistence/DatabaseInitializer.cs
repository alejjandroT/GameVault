using System.Reflection;
using Dapper;
using Microsoft.Data.Sqlite;

namespace GameVault.Infrastructure.Persistence;

public static class DatabaseInitializer
{
    public static void Initialize(string connectionString)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        var assembly = typeof(DatabaseInitializer).Assembly;
        var scripts = assembly
            .GetManifestResourceNames()
            .Where(name => name.EndsWith(".sql", StringComparison.OrdinalIgnoreCase))
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToList();

        foreach (var resourceName in scripts)
        {
            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream is null)
            {
                continue;
            }

            using var reader = new StreamReader(stream);
            var sql = reader.ReadToEnd();
            if (string.IsNullOrWhiteSpace(sql))
            {
                continue;
            }

            connection.Execute(sql);
        }
    }
}
