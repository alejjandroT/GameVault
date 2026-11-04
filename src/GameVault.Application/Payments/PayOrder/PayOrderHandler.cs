using GameVault.Application.Abstractions;
using GameVault.Application.Abstractions.Persistence;
using GameVault.Domain.Common;
using GameVault.Domain.Licenses;
using GameVault.Domain.Orders;
using GameVault.Domain.Payments;

namespace GameVault.Application.Payments.PayOrder;

public sealed class PayOrderHandler(
    IOrderRepository orderRepository,
    IPaymentRepository paymentRepository,
    IPaymentGateway paymentGateway)
{
    public async Task<Result<Payment>> HandleAsync(PayOrderCommand command, CancellationToken ct = default)
    {
        var order = await orderRepository.GetByIdAsync(command.OrderId, ct);
        if (order is null)
            return Result.Failure<Payment>(OrderErrors.NotFound(command.OrderId));

        var paymentResult = Payment.Create(order, order.Total, command.Method);
        if (paymentResult.IsFailure)
            return Result.Failure<Payment>(paymentResult.Error);

        var payment = paymentResult.Value;
        var approved = await paymentGateway.ChargeAsync(payment.Amount, payment.Method, command.Token, ct);
        if (!approved)
        {
            var rejectResult = payment.Reject();
            if (rejectResult.IsFailure)
                return Result.Failure<Payment>(rejectResult.Error);

            await paymentRepository.AddRejectedAsync(payment, ct);
            return Result.Failure<Payment>(PaymentErrors.Rejected);
        }

        var approveResult = payment.Approve();
        if (approveResult.IsFailure)
            return Result.Failure<Payment>(approveResult.Error);

        var markPaidResult = order.MarkAsPaid();
        if (markPaidResult.IsFailure)
            return Result.Failure<Payment>(markPaidResult.Error);

        var licenses = order.Items
            .Select(item => DigitalLicense.Issue(order.Id, order.ClientId, item.GameId))
            .ToList();
        await paymentRepository.AddApprovedAsync(payment, order, licenses, ct);

        var savedPayment = await paymentRepository.GetByIdAsync(payment.Id, ct);
        return savedPayment is null
            ? Result.Failure<Payment>(PaymentErrors.NotFound(payment.Id))
            : Result.Success(savedPayment);
    }
}