using GameVault.Application.Abstractions.Persistence;
using GameVault.Domain.Clients;
using GameVault.Domain.Common;
using GameVault.Domain.Licenses;

namespace GameVault.Application.Licenses.GetClientLibrary;

public sealed class GetClientLibraryHandler(IClientRepository clientRepository, ILicenseRepository licenseRepository)
{
    public async Task<Result<IReadOnlyList<DigitalLicense>>> HandleAsync(ClientId clientId, CancellationToken ct = default)
    {
        var client = await clientRepository.GetByIdAsync(clientId, ct);
        if (client is null)
            return Result.Failure<IReadOnlyList<DigitalLicense>>(ClientErrors.NotFound(clientId));

        var licenses = await licenseRepository.GetByClientIdAsync(clientId, ct);
        return Result.Success(licenses);
    }
}