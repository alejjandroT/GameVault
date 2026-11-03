using System.Data;
using System.Globalization;
using Dapper;
using GameVault.Application.Abstractions;
using GameVault.Application.Abstractions.Persistence;
using GameVault.Domain.Games;
using GameVault.Infrastructure.Persistence;
using GameVault.Infrastructure.Persistence.Registros;
using GameVault.Infrastructure.Persistence.Repositories;

namespace GameVault.Infrastructure.Games;

internal sealed class GameRepository : RepositoryBase, IGameRepository
{
    public GameRepository(IDbConnectionFactory connectionFactory, TimeProvider reloj, IUsuarioActual usuario)
        : base(connectionFactory, reloj, usuario)
    {
    }

    public async Task AddAsync(Game game, CancellationToken ct = default)
    {
        const string sql = @"
            INSERT INTO Games (Id, Title, Genre, Price, Status, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy)
            VALUES (@Id, @Title, @Genre, @Price, @Status, @CreatedAt, @CreatedBy, @UpdatedAt, @UpdatedBy);";

        using var connection = ConnectionFactory.CreateConnection();
        connection.Open();

        await connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                new
                {
                    Id = game.Id.Value.ToString(),
                    Title = game.Title,
                    Genre = game.Genre,
                    Price = game.Price.Amount,
                    Status = (int)game.Status,
                    CreatedAt = FormatDate(NowUtc),
                    CreatedBy = CurrentUser,
                    UpdatedAt = (string?)null,
                    UpdatedBy = game.UpdatedBy
                },
                cancellationToken: ct));
    }

    public async Task UpdateAsync(Game game, CancellationToken ct = default)
    {
        const string sql = @"
            UPDATE Games
            SET Title = @Title,
                Genre = @Genre,
                Price = @Price,
                Status = @Status,
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
                    Id = game.Id.Value.ToString(),
                    Title = game.Title,
                    Genre = game.Genre,
                    Price = game.Price.Amount,
                    Status = (int)game.Status,
                    UpdatedAt = FormatDate(NowUtc),
                    UpdatedBy = CurrentUser
                },
                cancellationToken: ct));
    }

    public async Task<Game?> GetByIdAsync(GameId id, CancellationToken ct = default)
    {
        const string sql = @"
            SELECT Id, Title, Genre, Price, Status, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy
            FROM Games
            WHERE Id = @Id;";

        using var connection = ConnectionFactory.CreateConnection();
        connection.Open();

        var registro = await connection.QuerySingleOrDefaultAsync<GameRegistro>(
            new CommandDefinition(sql, new { Id = id.Value.ToString() }, cancellationToken: ct));

        return registro?.ToEntity();
    }

    public async Task<IReadOnlyList<Game>> GetAllAsync(CancellationToken ct = default)
    {
        const string sql = @"
            SELECT Id, Title, Genre, Price, Status, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy
            FROM Games
            ORDER BY Title ASC;";

        using var connection = ConnectionFactory.CreateConnection();
        connection.Open();

        var registros = await connection.QueryAsync<GameRegistro>(
            new CommandDefinition(sql, cancellationToken: ct));

        return registros.Select(registro => registro.ToEntity()).ToList();
    }

    public async Task<bool> ExistsByTitleAsync(string title, GameId? excludeId = null, CancellationToken ct = default)
    {
        const string sql = @"
            SELECT 1
            FROM Games
                        WHERE Title = @Title
                            AND (@ExcludeId IS NULL OR Id <> @ExcludeId)
            LIMIT 1;";

        using var connection = ConnectionFactory.CreateConnection();
        connection.Open();

        var result = await connection.ExecuteScalarAsync<int?>(
            new CommandDefinition(sql, new { Title = title, ExcludeId = excludeId?.Value.ToString() }, cancellationToken: ct));

        return result.HasValue;
    }

    private static string FormatDate(DateTime value) => value
        .ToUniversalTime()
        .ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
}
