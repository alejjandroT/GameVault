using GameVault.Domain.Clients;

namespace GameVault.Application.Abstractions.Persistence;

public interface IClientRepository : IGenericRepository<Client, ClientId>
{
    Task<bool> ExistsByEmailAsync(string email, ClientId? excludeId = null, CancellationToken ct = default);
}