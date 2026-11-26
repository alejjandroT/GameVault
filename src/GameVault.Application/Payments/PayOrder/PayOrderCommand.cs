using GameVault.Domain.Orders;
using GameVault.Domain.Payments;

namespace GameVault.Application.Payments.PayOrder;

public sealed record PayOrderCommand(OrderId OrderId, PaymentMethod Method, string Token);