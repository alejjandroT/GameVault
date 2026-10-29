using System.Data;
using System.Globalization;
using Dapper;
using GameVault.Application.Games;
using GameVault.Domain.Games;
using GameVault.Infrastructure.Persistence;

namespace GameVault.Infrastructure.Games;

public sealed class GameRepository : IGameRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public GameRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task AddAsync(Game game, CancellationToken ct = default)
    {
        const string sql = @"
            INSERT INTO Games (Id, Title, Genre, Price, Status, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy)
            VALUES (@Id, @Title, @Genre, @Price, @Status, @CreatedAt, @CreatedBy, @UpdatedAt, @UpdatedBy);";

        using var connection = _connectionFactory.CreateConnection();
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
                    CreatedAt = FormatDate(game.CreatedAt),
                    CreatedBy = game.CreatedBy,
                    UpdatedAt = game.UpdatedAt.HasValue ? FormatDate(game.UpdatedAt.Value) : null,
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

        using var connection = _connectionFactory.CreateConnection();
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
                    UpdatedAt = game.UpdatedAt.HasValue ? FormatDate(game.UpdatedAt.Value) : null,
                    UpdatedBy = game.UpdatedBy
                },
                cancellationToken: ct));
    }

    public async Task<Game?> GetByIdAsync(GameId id, CancellationToken ct = default)
    {
        const string sql = @"
            SELECT Id, Title, Genre, Price, Status, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy
            FROM Games
            WHERE Id = @Id;";

        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        var row = await connection.QuerySingleOrDefaultAsync<GameRow>(
            new CommandDefinition(sql, new { Id = id.Value.ToString() }, cancellationToken: ct));

        return row is null ? null : Map(row);
    }

    public async Task<IReadOnlyList<Game>> GetAllAsync(CancellationToken ct = default)
    {
        const string sql = @"
            SELECT Id, Title, Genre, Price, Status, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy
            FROM Games
            ORDER BY Title ASC;";

        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        var rows = await connection.QueryAsync<GameRow>(
            new CommandDefinition(sql, cancellationToken: ct));

        return rows.Select(Map).ToList();
    }

    public async Task<bool> ExistsByTitleAsync(string title, GameId? excludeId = null, CancellationToken ct = default)
    {
        const string sql = @"
            SELECT 1
            FROM Games
            WHERE LOWER(Title) = LOWER(@Title)
                            AND (@ExcludeId IS NULL OR Id <> @ExcludeId)
            LIMIT 1;";

        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        var result = await connection.ExecuteScalarAsync<int?>(
            new CommandDefinition(sql, new { Title = title, ExcludeId = excludeId?.Value.ToString() }, cancellationToken: ct));

        return result.HasValue;
    }

    private static Game Map(GameRow row)
    {
        return Game.Reconstruct(
            new GameId(Guid.Parse(row.Id)),
            row.Title,
            row.Genre,
            row.Price,
            (GameStatus)row.Status,
            DateTime.Parse(row.CreatedAt, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind),
            row.CreatedBy,
            row.UpdatedAt is null ? null : DateTime.Parse(row.UpdatedAt, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind),
            row.UpdatedBy);
    }

    private static string FormatDate(DateTime value) => value.ToString("O", CultureInfo.InvariantCulture);

    private sealed record GameRow(
        string Id,
        string Title,
        string Genre,
        decimal Price,
        int Status,
        string CreatedAt,
        string CreatedBy,
        string? UpdatedAt,
        string? UpdatedBy);
}
