using GameVault.Domain.Payments;

namespace GameVault.Application.Abstractions;

public interface IPaymentGateway
{
    Task<bool> ChargeAsync(decimal amount, PaymentMethod method, string token, CancellationToken ct);
}