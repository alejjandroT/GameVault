using GameVault.Application.Abstractions.Persistence;
using GameVault.Domain.Clients;
using GameVault.Domain.Common;

namespace GameVault.Application.Clients.CreateClient;

public sealed class CreateClientHandler(IClientRepository repository)
{
    public async Task<Result<Client>> HandleAsync(CreateClientCommand command, CancellationToken ct = default)
    {
        var createResult = Client.Create(command.Name, command.Email);
        if (createResult.IsFailure)
            return Result.Failure<Client>(createResult.Error);

        var client = createResult.Value;
        var exists = await repository.ExistsByEmailAsync(client.Email.Value, ct: ct);
        if (exists)
            return Result.Failure<Client>(ClientErrors.DuplicateEmail(client.Email.Value));

        await repository.AddAsync(client, ct);
        var savedClient = await repository.GetByIdAsync(client.Id, ct);
        return savedClient is null
            ? Result.Failure<Client>(ClientErrors.NotFound(client.Id))
            : Result.Success(savedClient);
    }
}