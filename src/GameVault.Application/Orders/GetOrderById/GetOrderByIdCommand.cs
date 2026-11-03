using GameVault.Domain.Orders;

namespace GameVault.Application.Orders.GetOrderById;

public sealed record GetOrderByIdCommand(OrderId Id);