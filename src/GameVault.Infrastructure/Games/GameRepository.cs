using System.Data;
using Dapper;
using GameVault.Application.Games;
using GameVault.Domain.Games;
using GameVault.Infrastructure.Persistence;

namespace GameVault.Infrastructure.Games;

public sealed class GameRepository : IGameRepository
{
    private readonly ISqliteConnectionFactory _connectionFactory;

    public GameRepository(ISqliteConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task AddAsync(Game game, CancellationToken ct = default)
    {
        const string sql = @"
            INSERT INTO Games (Id, Title, Genre, Price, Status)
            VALUES (@Id, @Title, @Genre, @Price, @Status);";

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
                    Status = (int)game.Status
                },
                cancellationToken: ct));
    }

    public async Task<Game?> GetByIdAsync(GameId id, CancellationToken ct = default)
    {
        const string sql = @"
            SELECT Id, Title, Genre, Price, Status
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
            SELECT Id, Title, Genre, Price, Status
            FROM Games
            ORDER BY Title ASC;";

        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        var rows = await connection.QueryAsync<GameRow>(
            new CommandDefinition(sql, cancellationToken: ct));

        return rows.Select(Map).ToList();
    }

    public async Task<bool> ExistsByTitleAsync(string title, CancellationToken ct = default)
    {
        const string sql = @"
            SELECT 1
            FROM Games
            WHERE LOWER(Title) = LOWER(@Title)
            LIMIT 1;";

        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        var result = await connection.ExecuteScalarAsync<int?>(
            new CommandDefinition(sql, new { Title = title }, cancellationToken: ct));

        return result.HasValue;
    }

    private static Game Map(GameRow row)
    {
        return Game.Reconstruct(
            new GameId(Guid.Parse(row.Id)),
            row.Title,
            row.Genre,
            row.Price,
            (GameStatus)row.Status);
    }

    private sealed record GameRow(string Id, string Title, string Genre, decimal Price, int Status);
}
