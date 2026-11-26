using GameVault.Domain.Orders;

namespace GameVault.Application.Orders.CancelOrder;

public sealed record CancelOrderCommand(OrderId Id);