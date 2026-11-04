using Dapper;
using GameVault.Application.Abstractions.Persistence;
using GameVault.Domain.Clients;
using GameVault.Domain.Licenses;
using GameVault.Infrastructure.Persistence.Registros;

namespace GameVault.Infrastructure.Persistence.Repositories;

internal sealed class LicenseRepository : RepositoryBase, ILicenseRepository
{
    public LicenseRepository(
        GameVault.Infrastructure.Persistence.IDbConnectionFactory connectionFactory,
        TimeProvider reloj,
        GameVault.Application.Abstractions.IUsuarioActual usuario)
        : base(connectionFactory, reloj, usuario)
    {
    }

    public async Task<IReadOnlyList<DigitalLicense>> GetByClientIdAsync(ClientId clientId, CancellationToken ct = default)
    {
        const string sql = @"
            SELECT Id, OrderId, ClientId, GameId, LicenseKey, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy
            FROM DigitalLicenses
            WHERE ClientId = @ClientId
            ORDER BY CreatedAt DESC, Id ASC;";

        using var connection = ConnectionFactory.CreateConnection();
        connection.Open();

        var records = await connection.QueryAsync<DigitalLicenseRegistro>(
            new CommandDefinition(sql, new { ClientId = clientId.Value.ToString() }, cancellationToken: ct));
        return records.Select(record => record.ToEntity()).ToList();
    }
}