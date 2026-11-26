using GameVault.Application.Abstractions.Persistence;
using GameVault.Domain.Common;
using GameVault.Domain.Orders;

namespace GameVault.Application.Orders.CancelOrder;

public sealed class CancelOrderHandler(IOrderRepository repository)
{
    public async Task<Result> HandleAsync(CancelOrderCommand command, CancellationToken ct = default)
    {
        var order = await repository.GetByIdAsync(command.Id, ct);
        if (order is null)
            return Result.Failure(OrderErrors.NotFound(command.Id));

        var cancelResult = order.Cancel();
        if (cancelResult.IsFailure)
            return Result.Failure(cancelResult.Error);

        await repository.UpdateAsync(order, ct);
        return Result.Success();
    }
}