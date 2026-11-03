using GameVault.Domain.Games;
using GameVault.Domain.Orders;

namespace GameVault.Infrastructure.Persistence.Registros;

internal sealed class OrderItemRegistro
{
    public long Id { get; init; }
    public string OrderId { get; init; } = string.Empty;
    public string GameId { get; init; } = string.Empty;
    public decimal UnitPrice { get; init; }

    public OrderItem ToEntity() => OrderItem.Reconstruct(
        new GameId(Guid.Parse(GameId)),
        UnitPrice);
}