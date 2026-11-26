namespace GameVault.Domain.Orders;

public sealed record OrderId(Guid Value)
{
    public static OrderId New() => new(Guid.NewGuid());
}