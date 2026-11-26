using GameVault.Application.Abstractions.Persistence;
using GameVault.Application.Licenses;
using GameVault.Domain.Clients;
using GameVault.Domain.Common;
using GameVault.Domain.Licenses;

namespace GameVault.Application.Licenses.GetClientLibrary;

public sealed class GetClientLibraryHandler(IClientRepository clientRepository, ILicenseRepository licenseRepository)
{
    public async Task<Result<IReadOnlyList<LicenseDetail>>> HandleAsync(
        ClientId clientId,
        string? genre,
        CancellationToken ct = default)
    {
        var client = await clientRepository.GetByIdAsync(clientId, ct);
        if (client is null)
            return Result.Failure<IReadOnlyList<LicenseDetail>>(ClientErrors.NotFound(clientId));

        var licenses = await licenseRepository.GetLibraryAsync(clientId, genre, ct);
        return Result.Success(licenses);
    }
}