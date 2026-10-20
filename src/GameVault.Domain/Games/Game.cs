// GameVault.Domain/Games/Game.cs
using GameVault.Domain.Common;

namespace GameVault.Domain.Games;

public sealed class Game : Entity<GameId>
{
    public string Title { get; private set; }
    public string Genre { get; private set; }
    public Money Price { get; private set; }
    public GameStatus Status { get; private set; }

    private Game(GameId id, string title, string genre, Money price, GameStatus status)
        : base(id)
    {
        Title = title;
        Genre = genre;
        Price = price;
        Status = status;
    }

    public static Result<Game> Create(string title, string genre, decimal price)
    {
        var titleError = ValidateTitle(title);
        if (titleError is not null)
            return Result.Failure<Game>(titleError);

        if (string.IsNullOrWhiteSpace(genre))
            return Result.Failure<Game>(GameErrors.EmptyGenre);

        var priceResult = Money.Create(price);
        if (priceResult.IsFailure)
            return Result.Failure<Game>(priceResult.Error);

        var game = new Game(
            GameId.New(),
            title.Trim(),
            genre.Trim(),
            priceResult.Value,
            GameStatus.Draft);

        return Result.Success(game);
    }

    public static Game Reconstruct(GameId id, string title, string genre, decimal price, GameStatus status)
    {
        var moneyResult = Money.Create(price);
        if (moneyResult.IsFailure)
        {
            throw new InvalidOperationException($"No se puede reconstruir un juego inválido: {moneyResult.Error.Description}");
        }

        return new Game(id, title.Trim(), genre.Trim(), moneyResult.Value, status);
    }

    public Result Publish()
    {
        if (Status != GameStatus.Draft)
            return Result.Failure(GameErrors.CannotPublish);

        Status = GameStatus.Published;
        return Result.Success();
    }

    public Result Discontinue()
    {
        if (Status != GameStatus.Published)
            return Result.Failure(GameErrors.CannotDiscontinue);

        Status = GameStatus.Discontinued;
        return Result.Success();
    }

    public Result UpdatePrice(decimal newPrice)
    {
        if (Status == GameStatus.Discontinued)
            return Result.Failure(GameErrors.CannotChangeDiscontinued);

        var priceResult = Money.Create(newPrice);
        if (priceResult.IsFailure)
            return Result.Failure(priceResult.Error);

        Price = priceResult.Value;
        return Result.Success();
    }

    public Result UpdateDetails(string title, string genre)
    {
        var titleError = ValidateTitle(title);
        if (titleError is not null)
            return Result.Failure(titleError);

        if (string.IsNullOrWhiteSpace(genre))
            return Result.Failure(GameErrors.EmptyGenre);

        Title = title.Trim();
        Genre = genre.Trim();
        return Result.Success();
    }

    private static Error? ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return GameErrors.EmptyTitle;

        if (title.Length > 100)
            return GameErrors.TitleTooLong;

        return null;
    }
}
