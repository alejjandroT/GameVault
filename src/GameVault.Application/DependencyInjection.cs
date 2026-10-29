using GameVault.Application.Games.CreateGame;
using GameVault.Application.Games.GetGameById;
using GameVault.Application.Games.ListGames;
using GameVault.Application.Games.UpdateGame;
using GameVault.Application.Games.PublishGame;
using GameVault.Application.Games.DiscontinueGame;
using Microsoft.Extensions.DependencyInjection;

namespace GameVault.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<CreateGameHandler>();
        services.AddScoped<GetGameByIdHandler>();
        services.AddScoped<ListGamesHandler>();
        services.AddScoped<UpdateGameHandler>();
        services.AddScoped<PublishGameHandler>();
        services.AddScoped<DiscontinueGameHandler>();
        return services;
    }
}