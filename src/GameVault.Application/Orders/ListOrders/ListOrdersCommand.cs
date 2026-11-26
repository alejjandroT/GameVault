using GameVault.Domain.Clients;
using GameVault.Domain.Orders;

namespace GameVault.Application.Orders.ListOrders;

public sealed record ListOrdersCommand(ClientId? ClientId = null, OrderStatus? Status = null);