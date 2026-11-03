using System.Globalization;
using GameVault.Domain.Clients;

namespace GameVault.Infrastructure.Persistence.Registros;

internal sealed class ClientRegistro
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public int Active { get; init; }
    public string CreatedAt { get; init; } = string.Empty;
    public string CreatedBy { get; init; } = string.Empty;
    public string? UpdatedAt { get; init; }
    public string? UpdatedBy { get; init; }

    public Client ToEntity() => Client.Reconstruct(
        new ClientId(Guid.Parse(Id)),
        Name,
        Email,
        Active != 0,
        ParseUtc(CreatedAt),
        CreatedBy,
        UpdatedAt is null ? null : ParseUtc(UpdatedAt),
        UpdatedBy);

    private static DateTime ParseUtc(string value) => DateTime.SpecifyKind(
        DateTime.ParseExact(value, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
        DateTimeKind.Utc);
}