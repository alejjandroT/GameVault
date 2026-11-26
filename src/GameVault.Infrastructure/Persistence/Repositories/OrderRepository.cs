using System.Data;
using System.Globalization;
using Dapper;
using GameVault.Application.Abstractions;
using GameVault.Application.Abstractions.Persistence;
using GameVault.Application.Orders;
using GameVault.Domain.Clients;
using GameVault.Domain.Orders;
using GameVault.Infrastructure.Persistence.Registros;

namespace GameVault.Infrastructure.Persistence.Repositories;

internal sealed class OrderRepository : RepositoryBase, IOrderRepository
{
    public OrderRepository(IDbConnectionFactory connectionFactory, TimeProvider reloj, IUsuarioActual usuario)
        : base(connectionFactory, reloj, usuario)
    {
    }

    public async Task<Order?> GetByIdAsync(OrderId id, CancellationToken ct = default)
    {
        const string orderSql = @"
            SELECT Id, ClientId, Status, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy
            FROM Orders
            WHERE Id = @Id;";
        const string itemsSql = @"
            SELECT Id, OrderId, GameId, CAST(UnitPrice AS REAL) AS UnitPrice
            FROM OrderItems
            WHERE OrderId = @OrderId
            ORDER BY Id ASC;";

        using var connection = ConnectionFactory.CreateConnection();
        connection.Open();

        var record = await connection.QuerySingleOrDefaultAsync<OrderRegistro>(
            new CommandDefinition(orderSql, new { Id = id.Value.ToString() }, cancellationToken: ct));
        if (record is null)
            return null;

        var itemRecords = await connection.QueryAsync<OrderItemRegistro>(
            new CommandDefinition(itemsSql, new { OrderId = id.Value.ToString() }, cancellationToken: ct));

        return record.ToEntity(itemRecords.Select(item => item.ToEntity()).ToList());
    }

    public async Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken ct = default)
    {
        const string ordersSql = @"
            SELECT Id, ClientId, Status, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy
            FROM Orders
            ORDER BY CreatedAt DESC, Id ASC;";
        const string itemsSql = @"
            SELECT Id, OrderId, GameId, CAST(UnitPrice AS REAL) AS UnitPrice
            FROM OrderItems
            WHERE OrderId IN @OrderIds
            ORDER BY Id ASC;";

        using var connection = ConnectionFactory.CreateConnection();
        connection.Open();

        var records = (await connection.QueryAsync<OrderRegistro>(
            new CommandDefinition(ordersSql, cancellationToken: ct))).ToList();
        if (records.Count == 0)
            return [];

        var orderIds = records.Select(record => record.Id).ToArray();
        var itemRecords = await connection.QueryAsync<OrderItemRegistro>(
            new CommandDefinition(itemsSql, new { OrderIds = orderIds }, cancellationToken: ct));
        var itemsByOrder = itemRecords
            .GroupBy(item => item.OrderId)
            .ToDictionary(group => group.Key, group => (IReadOnlyList<GameVault.Domain.Orders.OrderItem>)group.Select(item => item.ToEntity()).ToList());

        return records
            .Select(record => record.ToEntity(itemsByOrder.GetValueOrDefault(record.Id, [])))
            .ToList();
    }

    public async Task<int> CountPendingAsync(ClientId clientId, DateTime sinceUtc, CancellationToken ct = default)
    {
        const string sql = @"
            SELECT COUNT(*)
            FROM Orders
            WHERE ClientId = @ClientId
              AND Status = 0
              AND CreatedAt > @SinceUtc;";

        using var connection = ConnectionFactory.CreateConnection();
        connection.Open();

        return await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                sql,
                new
                {
                    ClientId = clientId.Value.ToString(),
                    SinceUtc = FormatDate(sinceUtc)
                },
                cancellationToken: ct));
    }

    public async Task<IReadOnlyList<OrderSummary>> GetSummariesAsync(
        ClientId? clientId,
        OrderStatus? status,
        CancellationToken ct = default)
    {
        const string sql = @"
            SELECT
                o.Id,
                o.Status,
                (SELECT COALESCE(SUM(UnitPrice), 0)
                 FROM OrderItems
                 WHERE OrderId = o.Id) AS Total,
                (SELECT COUNT(*)
                 FROM OrderItems
                 WHERE OrderId = o.Id) AS ItemCount,
                o.CreatedAt,
                c.Id,
                c.Name,
                c.Email
            FROM Orders o
            INNER JOIN Clients c ON c.Id = o.ClientId
            WHERE (@ClientId IS NULL OR o.ClientId = @ClientId)
              AND (@Status IS NULL OR o.Status = @Status)
            ORDER BY o.CreatedAt DESC;";

        using var connection = ConnectionFactory.CreateConnection();
        connection.Open();

        var rows = await connection.QueryAsync<OrderSummaryFila, ClientSummaryFila, OrderSummary>(
            new CommandDefinition(
                sql,
                new
                {
                    ClientId = clientId?.Value.ToString(),
                    Status = status is null ? (int?)null : (int)status.Value
                },
                cancellationToken: ct),
            OrderSummaryMapper.Map,
            splitOn: "Id");

        return rows.ToList();
    }

    public async Task AddAsync(Order order, CancellationToken ct = default)
    {
        const string orderSql = @"
            INSERT INTO Orders (Id, ClientId, Status, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy)
            VALUES (@Id, @ClientId, @Status, @CreatedAt, @CreatedBy, @UpdatedAt, @UpdatedBy);";
        const string itemSql = @"
            INSERT INTO OrderItems (OrderId, GameId, UnitPrice)
            VALUES (@OrderId, @GameId, @UnitPrice);";

        using var connection = ConnectionFactory.CreateConnection();
        if (connection.State != ConnectionState.Open)
            connection.Open();

        using var transaction = connection.BeginTransaction();
        try
        {
            await connection.ExecuteAsync(
                new CommandDefinition(
                    orderSql,
                    new
                    {
                        Id = order.Id.Value.ToString(),
                        ClientId = order.ClientId.Value.ToString(),
                        Status = (int)order.Status,
                        CreatedAt = FormatDate(NowUtc),
                        CreatedBy = CurrentUser,
                        UpdatedAt = (string?)null,
                        UpdatedBy = (string?)null
                    },
                    transaction,
                    cancellationToken: ct));

            foreach (var item in order.Items)
            {
                await connection.ExecuteAsync(
                    new CommandDefinition(
                        itemSql,
                        new
                        {
                            OrderId = order.Id.Value.ToString(),
                            GameId = item.GameId.Value.ToString(),
                            UnitPrice = item.UnitPrice.Amount
                        },
                        transaction,
                        cancellationToken: ct));
            }

            transaction.Commit();
        }
        catch
        {
            try
            {
                transaction.Rollback();
            }
            catch
            {
                // Preserve the original persistence exception.
            }

            throw;
        }
    }

    public async Task UpdateAsync(Order order, CancellationToken ct = default)
    {
        const string sql = @"
            UPDATE Orders
            SET Status = @Status,
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
                    Id = order.Id.Value.ToString(),
                    Status = (int)order.Status,
                    UpdatedAt = FormatDate(NowUtc),
                    UpdatedBy = CurrentUser
                },
                cancellationToken: ct));
    }

    private static string FormatDate(DateTime value) => value
        .ToUniversalTime()
        .ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
}