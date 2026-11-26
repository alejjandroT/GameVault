using GameVault.Application.Abstractions.Persistence;
using GameVault.Domain.Common;
using GameVault.Domain.Orders;

namespace GameVault.Application.Orders.GetOrderById;

public sealed class GetOrderByIdHandler(IOrderRepository repository)
{
    public async Task<Result<Order>> HandleAsync(GetOrderByIdCommand command, CancellationToken ct = default)
    {
        var order = await repository.GetByIdAsync(command.Id, ct);
        return order is null
            ? Result.Failure<Order>(OrderErrors.NotFound(command.Id))
            : Result.Success(order);
    }
}