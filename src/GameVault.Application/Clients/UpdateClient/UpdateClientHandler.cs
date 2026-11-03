using GameVault.Application.Abstractions.Persistence;
using GameVault.Domain.Clients;
using GameVault.Domain.Common;

namespace GameVault.Application.Clients.UpdateClient;

public sealed class UpdateClientHandler(IClientRepository repository)
{
    public async Task<Result<Client>> HandleAsync(UpdateClientCommand command, CancellationToken ct = default)
    {
        var id = new ClientId(command.Id);
        var client = await repository.GetByIdAsync(id, ct);
        if (client is null)
            return Result.Failure<Client>(ClientErrors.NotFound(id));

        var emailResult = Email.Create(command.Email);
        if (emailResult.IsFailure)
            return Result.Failure<Client>(emailResult.Error);

        var exists = await repository.ExistsByEmailAsync(emailResult.Value.Value, id, ct);
        if (exists)
            return Result.Failure<Client>(ClientErrors.DuplicateEmail(emailResult.Value.Value));

        var nameResult = client.UpdateName(command.Name);
        if (nameResult.IsFailure)
            return Result.Failure<Client>(nameResult.Error);

        var updateEmailResult = client.UpdateEmail(command.Email);
        if (updateEmailResult.IsFailure)
            return Result.Failure<Client>(updateEmailResult.Error);

        await repository.UpdateAsync(client, ct);
        return Result.Success(client);
    }
}