namespace GameVault.Application.Orders;

public sealed record OrderSummary(
    Guid Id,
    string Status,
    decimal Total,
    int ItemCount,
    DateTime CreatedAt,
    ClientSummary Client);

public sealed record ClientSummary(Guid Id, string Name, string Email);