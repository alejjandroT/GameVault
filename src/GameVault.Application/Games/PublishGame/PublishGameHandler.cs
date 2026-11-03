using GameVault.Application.Abstractions.Persistence;
using GameVault.Domain.Common;
using GameVault.Domain.Games;

namespace GameVault.Application.Games.PublishGame;

public sealed class PublishGameHandler(IGameRepository repository)
{
    public async Task<Result<Game>> HandleAsync(GameId id, CancellationToken ct = default)
    {
        var game = await repository.GetByIdAsync(id, ct);
        if (game is null)
            return Result.Failure<Game>(GameErrors.NotFound(id));

        var publishResult = game.Publish();
        if (publishResult.IsFailure)
            return Result.Failure<Game>(publishResult.Error);

        await repository.UpdateAsync(game, ct);
        return Result.Success(game);
    }
}