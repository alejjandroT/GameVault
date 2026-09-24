
using GameVault.Domain.Games;

namespace GameVault.Application.Games;

public interface IGameRepository
{
    Task AddAsync(Game game, CancellationToken ct = default);
    Task<Game?> GetByIdAsync(GameId id, CancellationToken ct = default);
    Task<IReadOnlyList<Game>> GetAllAsync(CancellationToken ct = default);
    Task<bool> ExistsByTitleAsync(string title, CancellationToken ct = default);
}