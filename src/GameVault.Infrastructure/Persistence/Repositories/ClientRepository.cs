using System.Data;
using System.Globalization;
using Dapper;
using GameVault.Application.Abstractions;
using GameVault.Application.Abstractions.Persistence;
using GameVault.Domain.Clients;
using GameVault.Infrastructure.Persistence.Registros;

namespace GameVault.Infrastructure.Persistence.Repositories;

internal sealed class ClientRepository : RepositoryBase, IClientRepository
{
    public ClientRepository(IDbConnectionFactory connectionFactory, TimeProvider reloj, IUsuarioActual usuario)
        : base(connectionFactory, reloj, usuario)
    {
    }

    public async Task<IReadOnlyList<Client>> GetAllAsync(CancellationToken ct = default)
    {
        const string sql = @"
            SELECT Id, Name, Email, Active, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy
            FROM Clients
            ORDER BY Name ASC;";

        using var connection = ConnectionFactory.CreateConnection();
        connection.Open();

        var records = await connection.QueryAsync<ClientRegistro>(
            new CommandDefinition(sql, cancellationToken: ct));

        return records.Select(record => record.ToEntity()).ToList();
    }

    public async Task<Client?> GetByIdAsync(ClientId id, CancellationToken ct = default)
    {
        const string sql = @"
            SELECT Id, Name, Email, Active, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy
            FROM Clients
            WHERE Id = @Id;";

        using var connection = ConnectionFactory.CreateConnection();
        connection.Open();

        var record = await connection.QuerySingleOrDefaultAsync<ClientRegistro>(
            new CommandDefinition(sql, new { Id = id.Value.ToString() }, cancellationToken: ct));

        return record?.ToEntity();
    }

    public async Task<bool> ExistsByEmailAsync(string email, ClientId? excludeId = null, CancellationToken ct = default)
    {
        const string sql = @"
            SELECT 1
            FROM Clients
            WHERE Email = @Email
              AND (@ExcludeId IS NULL OR Id <> @ExcludeId)
            LIMIT 1;";

        using var connection = ConnectionFactory.CreateConnection();
        connection.Open();

        var result = await connection.ExecuteScalarAsync<int?>(
            new CommandDefinition(
                sql,
                new { Email = email, ExcludeId = excludeId?.Value.ToString() },
                cancellationToken: ct));

        return result.HasValue;
    }

    public async Task AddAsync(Client client, CancellationToken ct = default)
    {
        const string sql = @"
            INSERT INTO Clients (Id, Name, Email, Active, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy)
            VALUES (@Id, @Name, @Email, @Active, @CreatedAt, @CreatedBy, @UpdatedAt, @UpdatedBy);";

        using var connection = ConnectionFactory.CreateConnection();
        connection.Open();

        await connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                new
                {
                    Id = client.Id.Value.ToString(),
                    Name = client.Name,
                    Email = client.Email.Value,
                    Active = client.Active ? 1 : 0,
                    CreatedAt = FormatDate(NowUtc),
                    CreatedBy = CurrentUser,
                    UpdatedAt = (string?)null,
                    UpdatedBy = (string?)null
                },
                cancellationToken: ct));
    }

    public async Task UpdateAsync(Client client, CancellationToken ct = default)
    {
        const string sql = @"
            UPDATE Clients
            SET Name = @Name,
                Email = @Email,
                Active = @Active,
                UpdatedAt = @UpdatedAt,
                UpdatedBy = @UpdatedBy
            WHERE Id = @Id;";

        using var connection = ConnectionFactory.CreateConnection();
        connection.Open();

        await connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                new
                {
                    Id = client.Id.Value.ToString(),
                    Name = client.Name,
                    Email = client.Email.Value,
                    Active = client.Active ? 1 : 0,
                    UpdatedAt = FormatDate(NowUtc),
                    UpdatedBy = CurrentUser
                },
                cancellationToken: ct));
    }

    private static string FormatDate(DateTime value) => value
        .ToUniversalTime()
        .ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
}