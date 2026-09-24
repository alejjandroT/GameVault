using GameVault.Domain.Common;
using GameVault.Domain.Games;

namespace GameVault.Application.Games.CreateGame;

public sealed class CreateGameHandler(IGameRepository repository)
{
    public async Task<Result<Game>> HandleAsync(CreateGameCommand command, CancellationToken ct = default)
    {
        var createResult = Game.Create(command.Title, command.Genre, command.Price);
        if (createResult.IsFailure)
            return Result.Failure<Game>(createResult.Error);

        var exists = await repository.ExistsByTitleAsync(createResult.Value.Title, ct);
        if (exists)
            return Result.Failure<Game>(GameErrors.DuplicateTitle(createResult.Value.Title));

        await repository.AddAsync(createResult.Value, ct);
        return Result.Success(createResult.Value);
    }
}