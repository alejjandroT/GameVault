using GameVault.Application.Abstractions.Persistence;
using GameVault.Domain.Orders;

namespace GameVault.Application.Orders.ListOrders;

public sealed class ListOrdersHandler(IOrderRepository repository)
{
    public Task<IReadOnlyList<Order>> HandleAsync(ListOrdersCommand command, CancellationToken ct = default) =>
        repository.GetAllAsync(ct);
}