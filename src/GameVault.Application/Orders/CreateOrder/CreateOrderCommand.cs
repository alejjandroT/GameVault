using GameVault.Domain.Clients;
using GameVault.Domain.Games;

namespace GameVault.Application.Orders.CreateOrder;

public sealed record CreateOrderCommand(ClientId ClientId, IReadOnlyList<GameId> GameIds);