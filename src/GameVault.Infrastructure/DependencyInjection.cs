using GameVault.Application.Abstractions;
using GameVault.Application.Abstractions.Persistence;
using GameVault.Infrastructure.Games;
using GameVault.Infrastructure.Persistence;
using GameVault.Infrastructure.Servicios;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace GameVault.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var connectionString = configuration.GetConnectionString("GameVault");
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("No se encontró la cadena de conexión 'GameVault'.");

        services.AddSingleton<IDbConnectionFactory>(_ => new SqliteConnectionFactory(connectionString));
        services.TryAddSingleton<TimeProvider>(TimeProvider.System);
        services.AddScoped<IUsuarioActual, UsuarioActualSistema>();
        services.AddScoped<IGameRepository, GameRepository>();

        return services;
    }
}
