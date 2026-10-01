using GameVault.Domain.Common;
using GameVault.Domain.Games;

namespace GameVault.Application.Games.GetGameById;

public sealed class GetGameByIdHandler(IGameRepository repository)
{
    public async Task<Result<Game>> HandleAsync(GameId id, CancellationToken ct = default)
    {
        var game = await repository.GetByIdAsync(id, ct);
        return game is null
            ? Result.Failure<Game>(GameErrors.NotFound(id))
            : Result.Success(game);
    }
}