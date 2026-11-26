using GameVault.Application.Abstractions.Persistence;
using GameVault.Domain.Clients;
using GameVault.Domain.Common;

namespace GameVault.Application.Clients.GetClientById;

public sealed class GetClientByIdHandler(IClientRepository repository)
{
    public async Task<Result<Client>> HandleAsync(GetClientByIdCommand command, CancellationToken ct = default)
    {
        var client = await repository.GetByIdAsync(command.Id, ct);
        return client is null
            ? Result.Failure<Client>(ClientErrors.NotFound(command.Id))
            : Result.Success(client);
    }
}