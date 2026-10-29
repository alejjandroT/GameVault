using GameVault.Application.Common;
using GameVault.Domain.Common;
using GameVault.Domain.Games;

namespace GameVault.Application.Games.UpdateGame;

public sealed class UpdateGameHandler(IGameRepository repository)
{
    public async Task<Result<Game>> HandleAsync(UpdateGameCommand command, CancellationToken ct = default)
    {
        var id = new GameId(command.Id);
        var game = await repository.GetByIdAsync(id, ct);
        if (game is null)
            return Result.Failure<Game>(GameErrors.NotFound(id));

        var exists = await repository.ExistsByTitleAsync(command.Title, id, ct);
        if (exists)
            return Result.Failure<Game>(GameErrors.DuplicateTitle(command.Title));

        var detailsResult = game.UpdateDetails(command.Title, command.Genre);
        if (detailsResult.IsFailure)
            return Result.Failure<Game>(detailsResult.Error);

        var priceResult = game.UpdatePrice(command.Price);
        if (priceResult.IsFailure)
            return Result.Failure<Game>(priceResult.Error);

        game.MarkUpdated(AuditActor.System, DateTime.UtcNow);
        await repository.UpdateAsync(game, ct);
        return Result.Success(game);
    }
}