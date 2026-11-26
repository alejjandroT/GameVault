namespace GameVault.Application.Licenses;

public sealed record LicenseDetail(Guid Id, string Key, DateTime IssuedAt, GameSummary Game);

public sealed record GameSummary(Guid Id, string Title, string Genre);