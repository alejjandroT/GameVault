using GameVault.Application.Common;
using GameVault.Domain.Common;
using GameVault.Domain.Games;

namespace GameVault.Application.Games.DiscontinueGame;

public sealed class DiscontinueGameHandler(IGameRepository repository)
{
    public async Task<Result<Game>> HandleAsync(GameId id, CancellationToken ct = default)
    {
        var game = await repository.GetByIdAsync(id, ct);
        if (game is null)
            return Result.Failure<Game>(GameErrors.NotFound(id));

        var discontinueResult = game.Discontinue();
        if (discontinueResult.IsFailure)
            return Result.Failure<Game>(discontinueResult.Error);

        game.MarkUpdated(AuditActor.System, DateTime.UtcNow);
        await repository.UpdateAsync(game, ct);
        return Result.Success(game);
    }
}