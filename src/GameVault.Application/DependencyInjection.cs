using GameVault.Application.Games.CreateGame;
using GameVault.Application.Games.GetGameById;
using GameVault.Application.Games.ListGames;
using GameVault.Application.Games.UpdateGame;
using GameVault.Application.Games.PublishGame;
using GameVault.Application.Games.DiscontinueGame;
using GameVault.Application.Clients.CreateClient;
using GameVault.Application.Clients.DeactivateClient;
using GameVault.Application.Clients.GetClientById;
using GameVault.Application.Clients.ListClients;
using GameVault.Application.Clients.UpdateClient;
using GameVault.Application.Orders.CancelOrder;
using GameVault.Application.Orders.CreateOrder;
using GameVault.Application.Orders.GetOrderById;
using GameVault.Application.Orders.ListOrders;
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
        services.AddScoped<CreateClientHandler>();
        services.AddScoped<UpdateClientHandler>();
        services.AddScoped<DeactivateClientHandler>();
        services.AddScoped<GetClientByIdHandler>();
        services.AddScoped<ListClientsHandler>();
        services.AddScoped<CreateOrderHandler>();
        services.AddScoped<GetOrderByIdHandler>();
        services.AddScoped<ListOrdersHandler>();
        services.AddScoped<CancelOrderHandler>();
        return services;
    }
}