using GameVault.Application.Abstractions.Persistence;
using GameVault.Domain.Clients;
using GameVault.Domain.Common;
using GameVault.Domain.Games;
using GameVault.Domain.Orders;

namespace GameVault.Application.Orders.CreateOrder;

public sealed class CreateOrderHandler(
    IOrderRepository orderRepository,
    IClientRepository clientRepository,
    IGameRepository gameRepository)
{
    public async Task<Result<Order>> HandleAsync(CreateOrderCommand command, CancellationToken ct = default)
    {
        if (command.GameIds.Count == 0)
            return Result.Failure<Order>(OrderErrors.EmptyOrder);

        var client = await clientRepository.GetByIdAsync(command.ClientId, ct);
        if (client is null)
            return Result.Failure<Order>(ClientErrors.NotFound(command.ClientId));

        if (!client.Active)
            return Result.Failure<Order>(OrderErrors.ClientInactive);

        var orderResult = Order.Create(command.ClientId);
        var order = orderResult.Value;

        foreach (var gameId in command.GameIds)
        {
            var game = await gameRepository.GetByIdAsync(gameId, ct);
            if (game is null)
                return Result.Failure<Order>(GameErrors.NotFound(gameId));

            var addItemResult = order.AddItem(game);
            if (addItemResult.IsFailure)
                return Result.Failure<Order>(addItemResult.Error);
        }

        await orderRepository.AddAsync(order, ct);
        var savedOrder = await orderRepository.GetByIdAsync(order.Id, ct);
        return savedOrder is null
            ? Result.Failure<Order>(OrderErrors.NotFound(order.Id))
            : Result.Success(savedOrder);
    }
}