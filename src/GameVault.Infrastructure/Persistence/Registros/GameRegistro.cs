using System.Globalization;
using GameVault.Domain.Games;

namespace GameVault.Infrastructure.Persistence.Registros;

internal sealed class GameRegistro
{
    public string Id { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Genre { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public int Status { get; init; }
    public string CreatedAt { get; init; } = string.Empty;
    public string CreatedBy { get; init; } = string.Empty;
    public string? UpdatedAt { get; init; }
    public string? UpdatedBy { get; init; }

    public Game ToEntity()
    {
        return Game.Reconstruct(
            new GameId(Guid.Parse(Id)),
            Title,
            Genre,
            Price,
            (GameStatus)Status,
            ParseUtc(CreatedAt),
            CreatedBy,
            UpdatedAt is null ? null : ParseUtc(UpdatedAt),
            UpdatedBy);
    }

    private static DateTime ParseUtc(string value) => DateTime.SpecifyKind(
        DateTime.ParseExact(value, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
        DateTimeKind.Utc);
}