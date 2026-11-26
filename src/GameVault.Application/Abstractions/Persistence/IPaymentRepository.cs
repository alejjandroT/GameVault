using GameVault.Domain.Licenses;
using GameVault.Domain.Orders;
using GameVault.Domain.Payments;

namespace GameVault.Application.Abstractions.Persistence;

public interface IPaymentRepository : IGenericRepository<Payment, PaymentId>
{
    Task AddRejectedAsync(Payment payment, CancellationToken ct = default);
    Task AddApprovedAsync(Payment payment, Order order, IReadOnlyList<DigitalLicense> licenses, CancellationToken ct = default);
}