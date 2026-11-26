using GameVault.Domain.Clients;
using GameVault.Domain.Games;
using GameVault.Domain.Orders;
using GameVault.Domain.Payments;

namespace GameVault.Domain.Tests.Payments;

public sealed class PaymentTests
{
    private static readonly DateTime NowUtc = new(2026, 3, 10, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Create_ConMontoDistintoAlTotal_DebeFallarConAmountMismatch()
    {
        var order = CreateOrder(NowUtc.AddMinutes(-5));

        var result = Payment.Create(order, 49m, PaymentMethod.Card, NowUtc);

        Assert.True(result.IsFailure);
        Assert.Equal(PaymentErrors.AmountMismatch(order.Total, 49m), result.Error);
    }

    [Fact]
    public void Create_ConOrdenQueNoEstaPending_DebeFallar()
    {
        var order = CreateOrder(NowUtc.AddMinutes(-5), OrderStatus.Paid);

        var result = Payment.Create(order, order.Total, PaymentMethod.Card, NowUtc);

        Assert.True(result.IsFailure);
        Assert.Equal(PaymentErrors.OrderNotPayable, result.Error);
    }

    [Fact]
    public void Create_ConOrdenExpirada_DebeFallarAntesDeValidarElMonto()
    {
        var order = CreateOrder(NowUtc.AddMinutes(-31));

        var result = Payment.Create(order, 1m, PaymentMethod.Card, NowUtc);

        Assert.True(result.IsFailure);
        Assert.Equal(OrderErrors.Expired, result.Error);
    }

    [Fact]
    public void Create_ConOrdenVigenteYTotalExacto_DebeCrearPago()
    {
        var order = CreateOrder(NowUtc.AddMinutes(-5));

        var result = Payment.Create(order, order.Total, PaymentMethod.Wallet, NowUtc);

        Assert.True(result.IsSuccess);
        Assert.Equal(order.Id, result.Value.OrderId);
        Assert.Equal(order.Total, result.Value.Amount);
        Assert.Equal(PaymentMethod.Wallet, result.Value.Method);
    }

    private static Order CreateOrder(DateTime createdAt, OrderStatus status = OrderStatus.Pending) => Order.Reconstruct(
        new OrderId(Guid.Parse("dddddddd-dddd-4ddd-8ddd-dddddddddddd")),
        new ClientId(Guid.Parse("aaaaaaaa-aaaa-4aaa-8aaa-aaaaaaaaaaaa")),
        status,
        [OrderItem.Reconstruct(new GameId(Guid.Parse("22222222-2222-4222-8222-222222222222")), 50m)],
        createdAt,
        "unit-test",
        null,
        null);
}