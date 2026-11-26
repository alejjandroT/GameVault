using GameVault.Domain.Clients;
using GameVault.Domain.Games;
using GameVault.Domain.Orders;

namespace GameVault.Domain.Tests.Orders;

public sealed class OrderTests
{
    private static readonly DateTime CreatedAtUtc = new(2026, 3, 10, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void AddItem_ConJuegoPublished_DebeAgregarloYCongelarElPrecio()
    {
        var order = CreateOrder();
        var game = CreateGame("Juego publicado", 25m);
        Assert.True(game.Publish().IsSuccess);

        var result = order.AddItem(game);
        Assert.True(game.UpdatePrice(40m).IsSuccess);

        Assert.True(result.IsSuccess);
        Assert.Single(order.Items);
        Assert.Equal(25m, order.Items[0].UnitPrice.Amount);
        Assert.Equal(25m, order.Total);
    }

    [Fact]
    public void AddItem_ConJuegoDraft_DebeFallarConGameNotAvailable()
    {
        var order = CreateOrder();
        var game = CreateGame("Juego en borrador", 10m);

        var result = order.AddItem(game);

        Assert.True(result.IsFailure);
        Assert.Equal(OrderErrors.GameNotAvailable(game.Title), result.Error);
    }

    [Fact]
    public void AddItem_ConJuegoDiscontinued_DebeFallarConGameNotAvailable()
    {
        var order = CreateOrder();
        var game = CreateGame("Juego descontinuado", 10m);
        Assert.True(game.Publish().IsSuccess);
        Assert.True(game.Discontinue().IsSuccess);

        var result = order.AddItem(game);

        Assert.True(result.IsFailure);
        Assert.Equal(OrderErrors.GameNotAvailable(game.Title), result.Error);
    }

    [Fact]
    public void AddItem_ConElMismoJuegoDosVeces_DebeFallarConDuplicateGame()
    {
        var order = CreateOrder();
        var game = CreateGame("Juego publicado", 10m);
        Assert.True(game.Publish().IsSuccess);
        Assert.True(order.AddItem(game).IsSuccess);

        var result = order.AddItem(game);

        Assert.True(result.IsFailure);
        Assert.Equal(OrderErrors.DuplicateGame(game.Title), result.Error);
    }

    [Fact]
    public void Total_ConVariosItems_DebeSerLaSumaDeSusPrecios()
    {
        var order = CreateOrder();
        var firstGame = CreateGame("Primero", 12.50m);
        var secondGame = CreateGame("Segundo", 8.25m);
        Assert.True(firstGame.Publish().IsSuccess);
        Assert.True(secondGame.Publish().IsSuccess);

        Assert.True(order.AddItem(firstGame).IsSuccess);
        Assert.True(order.AddItem(secondGame).IsSuccess);

        Assert.Equal(20.75m, order.Total);
    }

    [Fact]
    public void Cancel_DesdePending_DebeCancelarYRechazarLaSegundaCancelacion()
    {
        var order = CreateOrder();

        var firstResult = order.Cancel();
        var secondResult = order.Cancel();

        Assert.True(firstResult.IsSuccess);
        Assert.Equal(OrderStatus.Cancelled, order.Status);
        Assert.True(secondResult.IsFailure);
        Assert.Equal(OrderErrors.CannotCancel, secondResult.Error);
    }

    [Fact]
    public void IsExpired_JustoAntesYDespuesDe30Minutos_DebeRespetarElLimite()
    {
        var order = ReconstructOrder(CreatedAtUtc);

        Assert.False(order.IsExpired(CreatedAtUtc.AddMinutes(30).AddTicks(-1)));
        Assert.False(order.IsExpired(CreatedAtUtc.AddMinutes(30)));
        Assert.True(order.IsExpired(CreatedAtUtc.AddMinutes(30).AddTicks(1)));
    }

    [Fact]
    public void IsExpired_EnEstadoNoPending_DebeSerFalso()
    {
        var order = ReconstructOrder(CreatedAtUtc, OrderStatus.Cancelled);

        Assert.False(order.IsExpired(CreatedAtUtc.AddHours(1)));
    }

    private static Order CreateOrder() => Order.Create(new ClientId(Guid.Parse("aaaaaaaa-aaaa-4aaa-8aaa-aaaaaaaaaaaa"))).Value;

    private static Order ReconstructOrder(DateTime createdAt, OrderStatus status = OrderStatus.Pending) => Order.Reconstruct(
        new OrderId(Guid.Parse("dddddddd-dddd-4ddd-8ddd-dddddddddddd")),
        new ClientId(Guid.Parse("aaaaaaaa-aaaa-4aaa-8aaa-aaaaaaaaaaaa")),
        status,
        [],
        createdAt,
        "unit-test",
        null,
        null);

    private static Game CreateGame(string title, decimal price) => Game.Create(title, "Aventura", price).Value;
}