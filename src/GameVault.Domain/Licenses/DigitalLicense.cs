using GameVault.Domain.Clients;
using GameVault.Domain.Common;
using GameVault.Domain.Games;
using GameVault.Domain.Orders;

namespace GameVault.Domain.Licenses;

public sealed class DigitalLicense : AuditableEntity<LicenseId>
{
    private DigitalLicense(LicenseId id, OrderId orderId, ClientId clientId, GameId gameId, string key)
        : base(id)
    {
        OrderId = orderId;
        ClientId = clientId;
        GameId = gameId;
        Key = key;
    }

    public OrderId OrderId { get; }
    public ClientId ClientId { get; }
    public GameId GameId { get; }
    public string Key { get; }

    public static DigitalLicense Issue(OrderId orderId, ClientId clientId, GameId gameId)
    {
        var rawKey = Guid.NewGuid().ToString("N").ToUpperInvariant();
        var key = $"{rawKey[..4]}-{rawKey[4..8]}-{rawKey[8..12]}-{rawKey[12..16]}";
        return new DigitalLicense(LicenseId.New(), orderId, clientId, gameId, key);
    }

    public static DigitalLicense Reconstruct(
        LicenseId id,
        OrderId orderId,
        ClientId clientId,
        GameId gameId,
        string key,
        DateTime createdAt,
        string createdBy,
        DateTime? updatedAt,
        string? updatedBy)
    {
        var license = new DigitalLicense(id, orderId, clientId, gameId, key);
        license.RestoreAudit(createdAt, createdBy, updatedAt, updatedBy);
        return license;
    }
}