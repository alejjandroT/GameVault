using GameVault.Domain.Games;

namespace GameVault.Domain.Tests.Games;

public sealed class GameTests
{
    [Fact]
    public void Create_ConDatosValidos_DebeNacerEnDraft()
    {
        var result = Game.Create("Nebula", "Aventura", 59.99m);

        Assert.True(result.IsSuccess);
        Assert.Equal(GameStatus.Draft, result.Value.Status);
    }

    [Fact]
    public void Create_ConTituloVacio_DebeFallar()
    {
        var result = Game.Create("  ", "Aventura", 10m);

        Assert.True(result.IsFailure);
        Assert.Equal(GameErrors.EmptyTitle, result.Error);
    }

    [Fact]
    public void Create_ConTituloMayorA100Caracteres_DebeFallar()
    {
        var result = Game.Create(new string('A', 101), "Aventura", 10m);

        Assert.True(result.IsFailure);
        Assert.Equal(GameErrors.TitleTooLong, result.Error);
    }

    [Fact]
    public void Create_ConPrecioNegativo_DebeFallar()
    {
        var result = Game.Create("Nebula", "Aventura", -0.01m);

        Assert.True(result.IsFailure);
        Assert.Equal(GameErrors.NegativePrice, result.Error);
    }

    [Fact]
    public void Create_ConPrecioMayorA500_DebeFallar()
    {
        var result = Game.Create("Nebula", "Aventura", 500.01m);

        Assert.True(result.IsFailure);
        Assert.Equal(GameErrors.PriceExceedsMax, result.Error);
    }

    [Fact]
    public void Publish_DesdeDraft_DebePublicar()
    {
        var game = CreateGame();

        var result = game.Publish();

        Assert.True(result.IsSuccess);
        Assert.Equal(GameStatus.Published, game.Status);
    }

    [Fact]
    public void Publish_Repetido_DebeFallar()
    {
        var game = CreateGame();
        Assert.True(game.Publish().IsSuccess);

        var result = game.Publish();

        Assert.True(result.IsFailure);
        Assert.Equal(GameErrors.CannotPublish, result.Error);
    }

    [Fact]
    public void Discontinue_DesdePublished_DebeDescontinuar()
    {
        var game = CreateGame();
        Assert.True(game.Publish().IsSuccess);

        var result = game.Discontinue();

        Assert.True(result.IsSuccess);
        Assert.Equal(GameStatus.Discontinued, game.Status);
    }

    [Fact]
    public void Discontinue_DesdeDraft_DebeFallar()
    {
        var game = CreateGame();

        var result = game.Discontinue();

        Assert.True(result.IsFailure);
        Assert.Equal(GameErrors.CannotDiscontinue, result.Error);
    }

    [Fact]
    public void UpdatePrice_EnJuegoDiscontinued_DebeFallar()
    {
        var game = CreateGame();
        Assert.True(game.Publish().IsSuccess);
        Assert.True(game.Discontinue().IsSuccess);

        var result = game.UpdatePrice(20m);

        Assert.True(result.IsFailure);
        Assert.Equal(GameErrors.CannotChangeDiscontinued, result.Error);
        Assert.Equal(10m, game.Price.Amount);
    }

    private static Game CreateGame() => Game.Create("Nebula", "Aventura", 10m).Value;
}