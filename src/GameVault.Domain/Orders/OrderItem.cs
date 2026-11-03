using GameVault.Domain.Games;

namespace GameVault.Domain.Orders;

public sealed class OrderItem
{
    private OrderItem(GameId gameId, Money unitPrice)
    {
        GameId = gameId;
        UnitPrice = unitPrice;
    }

    public GameId GameId { get; }
    public Money UnitPrice { get; }

    internal static OrderItem Create(GameId gameId, Money unitPrice) => new(gameId, unitPrice);

    public static OrderItem Reconstruct(GameId gameId, decimal unitPrice)
    {
        var priceResult = Money.Create(unitPrice);
        if (priceResult.IsFailure)
            throw new InvalidOperationException($"No se puede reconstruir un ítem de orden inválido: {priceResult.Error.Description}");

        return new OrderItem(gameId, priceResult.Value);
    }
}