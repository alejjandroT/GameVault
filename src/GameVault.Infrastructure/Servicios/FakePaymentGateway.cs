using GameVault.Application.Abstractions;
using GameVault.Domain.Payments;

namespace GameVault.Infrastructure.Servicios;

internal sealed class FakePaymentGateway : IPaymentGateway
{
    // Esta es una simulación; en producción se reemplazaría por Stripe u otro proveedor de pagos.
    public Task<bool> ChargeAsync(decimal amount, PaymentMethod method, string token, CancellationToken ct) =>
        Task.FromResult(token != "tok_fail");
}