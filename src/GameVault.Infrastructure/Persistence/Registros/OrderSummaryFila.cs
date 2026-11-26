using System.Globalization;
using GameVault.Application.Orders;
using GameVault.Domain.Orders;

namespace GameVault.Infrastructure.Persistence.Registros;

internal sealed record OrderSummaryFila
{
    public string Id { get; init; } = string.Empty;
    public int Status { get; init; }
    public decimal Total { get; init; }
    public int ItemCount { get; init; }
    public string CreatedAt { get; init; } = string.Empty;
}

internal sealed record ClientSummaryFila
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
}

internal static class OrderSummaryMapper
{
    public static OrderSummary Map(OrderSummaryFila order, ClientSummaryFila client) => new(
        Guid.Parse(order.Id),
        ((OrderStatus)order.Status).ToString(),
        order.Total,
        order.ItemCount,
        DateTime.SpecifyKind(
            DateTime.ParseExact(order.CreatedAt, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
            DateTimeKind.Utc),
        new ClientSummary(Guid.Parse(client.Id), client.Name, client.Email));
}