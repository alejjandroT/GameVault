namespace GameVault.Domain.Clients;

public sealed record ClientId(Guid Value)
{
    public static ClientId New() => new(Guid.NewGuid());
}