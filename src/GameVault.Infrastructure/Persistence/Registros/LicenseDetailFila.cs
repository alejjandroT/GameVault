using System.Globalization;
using GameVault.Application.Licenses;

namespace GameVault.Infrastructure.Persistence.Registros;

internal sealed record LicenseDetailFila
{
    public string Id { get; init; } = string.Empty;
    public string LicenseKey { get; init; } = string.Empty;
    public string CreatedAt { get; init; } = string.Empty;
}

internal sealed record GameSummaryFila
{
    public string Id { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Genre { get; init; } = string.Empty;
}

internal static class LicenseDetailMapper
{
    public static LicenseDetail Map(LicenseDetailFila license, GameSummaryFila game) => new(
        Guid.Parse(license.Id),
        license.LicenseKey,
        DateTime.SpecifyKind(
            DateTime.ParseExact(license.CreatedAt, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
            DateTimeKind.Utc),
        new GameSummary(Guid.Parse(game.Id), game.Title, game.Genre));
}