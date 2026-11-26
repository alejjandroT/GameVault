using GameVault.Application.Orders;
using GameVault.Domain.Clients;
using GameVault.Domain.Orders;

namespace GameVault.Application.Abstractions.Persistence;

public interface IOrderRepository : IGenericRepository<Order, OrderId>
{
	Task<int> CountPendingAsync(ClientId clientId, DateTime sinceUtc, CancellationToken ct = default);

	Task<IReadOnlyList<OrderSummary>> GetSummariesAsync(
		ClientId? clientId,
		OrderStatus? status,
		CancellationToken ct = default);
}