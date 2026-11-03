using GameVault.Application.Abstractions.Persistence;
using GameVault.Domain.Clients;

namespace GameVault.Application.Clients.ListClients;

public sealed class ListClientsHandler(IClientRepository repository)
{
    public Task<IReadOnlyList<Client>> HandleAsync(ListClientsCommand command, CancellationToken ct = default) =>
        repository.GetAllAsync(ct);
}