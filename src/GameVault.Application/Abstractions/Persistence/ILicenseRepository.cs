using GameVault.Domain.Clients;
using GameVault.Application.Licenses;

namespace GameVault.Application.Abstractions.Persistence;

public interface ILicenseRepository
{
    Task<IReadOnlyList<LicenseDetail>> GetLibraryAsync(
        ClientId clientId,
        string? genre,
        CancellationToken ct = default);
}