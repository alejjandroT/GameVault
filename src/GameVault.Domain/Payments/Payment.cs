using GameVault.Domain.Common;
using GameVault.Domain.Orders;

namespace GameVault.Domain.Payments;

public sealed class Payment : AuditableEntity<PaymentId>
{
    private bool _finalized;

    private Payment(PaymentId id, OrderId orderId, decimal amount, PaymentMethod method, PaymentStatus status)
        : base(id)
    {
        OrderId = orderId;
        Amount = amount;
        Method = method;
        Status = status;
    }

    public OrderId OrderId { get; }
    public decimal Amount { get; }
    public PaymentMethod Method { get; }
    public PaymentStatus Status { get; private set; }

    public static Result<Payment> Create(Order order, decimal amount, PaymentMethod method)
    {
        if (order.Status != OrderStatus.Pending)
            return Result.Failure<Payment>(PaymentErrors.OrderNotPayable);

        if (amount != order.Total)
            return Result.Failure<Payment>(PaymentErrors.AmountMismatch(order.Total, amount));

        return Result.Success(new Payment(PaymentId.New(), order.Id, amount, method, PaymentStatus.Rejected));
    }

    public Result Approve()
    {
        if (_finalized)
            return Result.Failure(PaymentErrors.AlreadyFinalized);

        Status = PaymentStatus.Approved;
        _finalized = true;
        return Result.Success();
    }

    public Result Reject()
    {
        if (_finalized)
            return Result.Failure(PaymentErrors.AlreadyFinalized);

        Status = PaymentStatus.Rejected;
        _finalized = true;
        return Result.Success();
    }

    public static Payment Reconstruct(
        PaymentId id,
        OrderId orderId,
        decimal amount,
        PaymentMethod method,
        PaymentStatus status,
        DateTime createdAt,
        string createdBy,
        DateTime? updatedAt,
        string? updatedBy)
    {
        var payment = new Payment(id, orderId, amount, method, status) { _finalized = true };
        payment.RestoreAudit(createdAt, createdBy, updatedAt, updatedBy);
        return payment;
    }
}