
namespace GameVault.Domain.Games;

public sealed record GameId(Guid Value)
{
    public static GameId New() => new(Guid.NewGuid());
} 