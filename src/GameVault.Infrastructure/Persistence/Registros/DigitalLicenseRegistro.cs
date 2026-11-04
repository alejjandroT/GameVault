using System.Globalization;
using GameVault.Domain.Clients;
using GameVault.Domain.Games;
using GameVault.Domain.Licenses;
using GameVault.Domain.Orders;

namespace GameVault.Infrastructure.Persistence.Registros;

internal sealed class DigitalLicenseRegistro
{
    public string Id { get; init; } = string.Empty;
    public string OrderId { get; init; } = string.Empty;
    public string ClientId { get; init; } = string.Empty;
    public string GameId { get; init; } = string.Empty;
    public string LicenseKey { get; init; } = string.Empty;
    public string CreatedAt { get; init; } = string.Empty;
    public string CreatedBy { get; init; } = string.Empty;
    public string? UpdatedAt { get; init; }
    public string? UpdatedBy { get; init; }

    public DigitalLicense ToEntity() => DigitalLicense.Reconstruct(
        new LicenseId(Guid.Parse(Id)),
        new OrderId(Guid.Parse(OrderId)),
        new ClientId(Guid.Parse(ClientId)),
        new GameId(Guid.Parse(GameId)),
        LicenseKey,
        ParseUtc(CreatedAt),
        CreatedBy,
        UpdatedAt is null ? null : ParseUtc(UpdatedAt),
        UpdatedBy);

    private static DateTime ParseUtc(string value) => DateTime.SpecifyKind(
        DateTime.ParseExact(value, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
        DateTimeKind.Utc);
}