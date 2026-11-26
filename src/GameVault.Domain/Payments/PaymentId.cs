namespace GameVault.Domain.Payments;

public sealed record PaymentId(Guid Value)
{
    public static PaymentId New() => new(Guid.NewGuid());
}