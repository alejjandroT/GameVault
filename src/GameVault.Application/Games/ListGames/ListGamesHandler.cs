using GameVault.Domain.Games;

namespace GameVault.Application.Games.ListGames;

public sealed class ListGamesHandler(IGameRepository repository)
{
    public Task<IReadOnlyList<Game>> HandleAsync(CancellationToken ct = default) =>
        repository.GetAllAsync(ct);
}