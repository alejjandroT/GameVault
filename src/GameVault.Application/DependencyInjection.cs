using GameVault.Application.Games.CreateGame;
using GameVault.Application.Games.GetGameById;
using GameVault.Application.Games.ListGames;
using Microsoft.Extensions.DependencyInjection;

namespace GameVault.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<CreateGameHandler>();
        services.AddScoped<GetGameByIdHandler>();
        services.AddScoped<ListGamesHandler>();
        return services;
    }
}