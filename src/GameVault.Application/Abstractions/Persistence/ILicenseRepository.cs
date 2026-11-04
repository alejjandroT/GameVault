using GameVault.Domain.Clients;
using GameVault.Domain.Licenses;

namespace GameVault.Application.Abstractions.Persistence;

public interface ILicenseRepository
{
    Task<IReadOnlyList<DigitalLicense>> GetByClientIdAsync(ClientId clientId, CancellationToken ct = default);
}