using System.Globalization;
using GameVault.Domain.Orders;
using GameVault.Domain.Payments;

namespace GameVault.Infrastructure.Persistence.Registros;

internal sealed class PaymentRegistro
{
    public string Id { get; init; } = string.Empty;
    public string OrderId { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public int Method { get; init; }
    public int Status { get; init; }
    public string CreatedAt { get; init; } = string.Empty;
    public string CreatedBy { get; init; } = string.Empty;
    public string? UpdatedAt { get; init; }
    public string? UpdatedBy { get; init; }

    public Payment ToEntity() => Payment.Reconstruct(
        new PaymentId(Guid.Parse(Id)),
        new OrderId(Guid.Parse(OrderId)),
        Amount,
        (PaymentMethod)Method,
        (PaymentStatus)Status,
        ParseUtc(CreatedAt),
        CreatedBy,
        UpdatedAt is null ? null : ParseUtc(UpdatedAt),
        UpdatedBy);

    private static DateTime ParseUtc(string value) => DateTime.SpecifyKind(
        DateTime.ParseExact(value, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
        DateTimeKind.Utc);
}