using System.Globalization;
using GameVault.Domain.Clients;
using GameVault.Domain.Orders;

namespace GameVault.Infrastructure.Persistence.Registros;

internal sealed class OrderRegistro
{
    public string Id { get; init; } = string.Empty;
    public string ClientId { get; init; } = string.Empty;
    public int Status { get; init; }
    public string CreatedAt { get; init; } = string.Empty;
    public string CreatedBy { get; init; } = string.Empty;
    public string? UpdatedAt { get; init; }
    public string? UpdatedBy { get; init; }

    public Order ToEntity(IReadOnlyList<OrderItem> items) => Order.Reconstruct(
        new OrderId(Guid.Parse(Id)),
        new ClientId(Guid.Parse(ClientId)),
        (OrderStatus)Status,
        items,
        ParseUtc(CreatedAt),
        CreatedBy,
        UpdatedAt is null ? null : ParseUtc(UpdatedAt),
        UpdatedBy);

    private static DateTime ParseUtc(string value) => DateTime.SpecifyKind(
        DateTime.ParseExact(value, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
        DateTimeKind.Utc);
}