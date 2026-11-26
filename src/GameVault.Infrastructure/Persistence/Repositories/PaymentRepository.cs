using System.Data;
using System.Globalization;
using Dapper;
using GameVault.Application.Abstractions;
using GameVault.Application.Abstractions.Persistence;
using GameVault.Domain.Licenses;
using GameVault.Domain.Orders;
using GameVault.Domain.Payments;
using GameVault.Infrastructure.Persistence.Registros;

namespace GameVault.Infrastructure.Persistence.Repositories;

internal sealed class PaymentRepository : RepositoryBase, IPaymentRepository
{
    public PaymentRepository(IDbConnectionFactory connectionFactory, TimeProvider reloj, IUsuarioActual usuario)
        : base(connectionFactory, reloj, usuario)
    {
    }

    public async Task<IReadOnlyList<Payment>> GetAllAsync(CancellationToken ct = default)
    {
        const string sql = @"
            SELECT Id, OrderId, CAST(Amount AS REAL) AS Amount, Method, Status, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy
            FROM Payments
            ORDER BY CreatedAt DESC, Id ASC;";

        using var connection = ConnectionFactory.CreateConnection();
        connection.Open();

        var records = await connection.QueryAsync<PaymentRegistro>(
            new CommandDefinition(sql, cancellationToken: ct));
        return records.Select(record => record.ToEntity()).ToList();
    }

    public async Task<Payment?> GetByIdAsync(PaymentId id, CancellationToken ct = default)
    {
        const string sql = @"
            SELECT Id, OrderId, CAST(Amount AS REAL) AS Amount, Method, Status, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy
            FROM Payments
            WHERE Id = @Id;";

        using var connection = ConnectionFactory.CreateConnection();
        connection.Open();

        var record = await connection.QuerySingleOrDefaultAsync<PaymentRegistro>(
            new CommandDefinition(sql, new { Id = id.Value.ToString() }, cancellationToken: ct));
        return record?.ToEntity();
    }

    public Task AddAsync(Payment payment, CancellationToken ct = default) => AddRejectedAsync(payment, ct);

    public async Task AddRejectedAsync(Payment payment, CancellationToken ct = default)
    {
        const string sql = @"
            INSERT INTO Payments (Id, OrderId, Amount, Method, Status, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy)
            VALUES (@Id, @OrderId, @Amount, @Method, @Status, @CreatedAt, @CreatedBy, @UpdatedAt, @UpdatedBy);";

        using var connection = ConnectionFactory.CreateConnection();
        connection.Open();

        await connection.ExecuteAsync(
            new CommandDefinition(sql, PaymentParameters(payment, NowUtc), cancellationToken: ct));
    }

    public async Task AddApprovedAsync(
        Payment payment,
        Order order,
        IReadOnlyList<DigitalLicense> licenses,
        CancellationToken ct = default)
    {
        const string paymentSql = @"
            INSERT INTO Payments (Id, OrderId, Amount, Method, Status, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy)
            VALUES (@Id, @OrderId, @Amount, @Method, @Status, @CreatedAt, @CreatedBy, @UpdatedAt, @UpdatedBy);";
        const string orderSql = @"
            UPDATE Orders
            SET Status = @Status,
                UpdatedAt = @UpdatedAt,
                UpdatedBy = @UpdatedBy
            WHERE Id = @Id;";
        const string licenseSql = @"
            INSERT INTO DigitalLicenses
                (Id, OrderId, ClientId, GameId, LicenseKey, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy)
            VALUES
                (@Id, @OrderId, @ClientId, @GameId, @LicenseKey, @CreatedAt, @CreatedBy, @UpdatedAt, @UpdatedBy);";

        using var connection = ConnectionFactory.CreateConnection();
        if (connection.State != ConnectionState.Open)
            connection.Open();

        using var transaction = connection.BeginTransaction();
        try
        {
            var now = NowUtc;
            await connection.ExecuteAsync(
                new CommandDefinition(paymentSql, PaymentParameters(payment, now), transaction, cancellationToken: ct));

            await connection.ExecuteAsync(
                new CommandDefinition(
                    orderSql,
                    new
                    {
                        Id = order.Id.Value.ToString(),
                        Status = (int)order.Status,
                        UpdatedAt = FormatDate(now),
                        UpdatedBy = CurrentUser
                    },
                    transaction,
                    cancellationToken: ct));

            foreach (var license in licenses)
            {
                await connection.ExecuteAsync(
                    new CommandDefinition(
                        licenseSql,
                        new
                        {
                            Id = license.Id.Value.ToString(),
                            OrderId = license.OrderId.Value.ToString(),
                            ClientId = license.ClientId.Value.ToString(),
                            GameId = license.GameId.Value.ToString(),
                            LicenseKey = license.Key,
                            CreatedAt = FormatDate(now),
                            CreatedBy = CurrentUser,
                            UpdatedAt = (string?)null,
                            UpdatedBy = (string?)null
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

    public async Task UpdateAsync(Payment payment, CancellationToken ct = default)
    {
        const string sql = @"
            UPDATE Payments
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
                    Id = payment.Id.Value.ToString(),
                    Status = (int)payment.Status,
                    UpdatedAt = FormatDate(NowUtc),
                    UpdatedBy = CurrentUser
                },
                cancellationToken: ct));
    }

    private object PaymentParameters(Payment payment, DateTime createdAt) => new
    {
        Id = payment.Id.Value.ToString(),
        OrderId = payment.OrderId.Value.ToString(),
        payment.Amount,
        Method = (int)payment.Method,
        Status = (int)payment.Status,
        CreatedAt = FormatDate(createdAt),
        CreatedBy = CurrentUser,
        UpdatedAt = (string?)null,
        UpdatedBy = (string?)null
    };

    private static string FormatDate(DateTime value) => value
        .ToUniversalTime()
        .ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
}