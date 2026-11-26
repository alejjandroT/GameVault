namespace GameVault.Domain.Licenses;

public sealed record LicenseId(Guid Value)
{
    public static LicenseId New() => new(Guid.NewGuid());
}