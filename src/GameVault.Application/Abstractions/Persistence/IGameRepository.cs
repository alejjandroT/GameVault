using GameVault.Domain.Games;

namespace GameVault.Application.Abstractions.Persistence;

public interface IGameRepository : IGenericRepository<Game, GameId>
{
    Task<bool> ExistsByTitleAsync(string title, GameId? excludeId = null, CancellationToken ct = default);
}