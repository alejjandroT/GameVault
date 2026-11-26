using Dapper;
using GameVault.Application.Abstractions.Persistence;
using GameVault.Application.Licenses;
using GameVault.Domain.Clients;
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

    public async Task<IReadOnlyList<LicenseDetail>> GetLibraryAsync(
        ClientId clientId,
        string? genre,
        CancellationToken ct = default)
    {
        const string sql = @"
            SELECT
                d.Id,
                d.LicenseKey,
                d.CreatedAt,
                g.Id,
                g.Title,
                g.Genre
            FROM DigitalLicenses d
            INNER JOIN Games g ON g.Id = d.GameId
            WHERE d.ClientId = @ClientId
              AND (@Genre IS NULL OR g.Genre = @Genre COLLATE NOCASE)
            ORDER BY d.CreatedAt DESC;";

        using var connection = ConnectionFactory.CreateConnection();
        connection.Open();

        var rows = await connection.QueryAsync<LicenseDetailFila, GameSummaryFila, LicenseDetail>(
            new CommandDefinition(
                sql,
                new { ClientId = clientId.Value.ToString(), Genre = genre },
                cancellationToken: ct),
            LicenseDetailMapper.Map,
            splitOn: "Id");
        return rows.ToList();
    }
}