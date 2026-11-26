using GameVault.Application.Abstractions.Persistence;
using GameVault.Application.Orders;

namespace GameVault.Application.Orders.ListOrders;

public sealed class ListOrdersHandler(IOrderRepository repository)
{
    public Task<IReadOnlyList<OrderSummary>> HandleAsync(ListOrdersCommand command, CancellationToken ct = default) =>
        repository.GetSummariesAsync(command.ClientId, command.Status, ct);
}