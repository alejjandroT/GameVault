using GameVault.Domain.Orders;

namespace GameVault.Application.Abstractions.Persistence;

public interface IOrderRepository : IGenericRepository<Order, OrderId>
{
}