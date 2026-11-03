using GameVault.Application.Abstractions.Persistence;
using GameVault.Domain.Clients;
using GameVault.Domain.Common;

namespace GameVault.Application.Clients.DeactivateClient;

public sealed class DeactivateClientHandler(IClientRepository repository)
{
    public async Task<Result> HandleAsync(DeactivateClientCommand command, CancellationToken ct = default)
    {
        var id = new ClientId(command.Id);
        var client = await repository.GetByIdAsync(id, ct);
        if (client is null)
            return Result.Failure(ClientErrors.NotFound(id));

        var deactivateResult = client.Deactivate();
        if (deactivateResult.IsFailure)
            return Result.Failure(deactivateResult.Error);

        await repository.UpdateAsync(client, ct);
        return Result.Success();
    }
}