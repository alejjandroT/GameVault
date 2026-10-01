using GameVault.Application.Games;
using GameVault.Infrastructure.Games;
using GameVault.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GameVault.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var connectionString = configuration.GetConnectionString("Sqlite") ?? "Data Source=gamevault.db";

        DatabaseInitializer.Initialize(connectionString);

        services.AddSingleton<ISqliteConnectionFactory>(_ => new SqliteConnectionFactory(connectionString));
        services.AddScoped<IGameRepository, GameRepository>();

        return services;
    }
}
