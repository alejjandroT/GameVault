using GameVault.Domain.Common;
using GameVault.Domain.Orders;

namespace GameVault.Domain.Payments;

public static class PaymentErrors
{
    public static readonly Error OrderNotPayable = Error.Conflict(
        "Payment.OrderNotPayable", "La orden no está disponible para pago.");

    public static Error AmountMismatch(decimal expected, decimal received) => Error.Conflict(
        "Payment.AmountMismatch", $"El monto del pago ({received}) no coincide con el total de la orden ({expected}).");

    public static readonly Error Rejected = Error.Conflict(
        "Payment.Rejected", "El proveedor de pagos rechazó la transacción.");

    public static readonly Error AlreadyFinalized = Error.Conflict(
        "Payment.AlreadyFinalized", "El pago ya fue confirmado y no puede cambiar de estado.");

    public static Error NotFound(PaymentId id) => Error.NotFound(
        "Payment.NotFound", $"No se encontró un pago con el Id '{id.Value}'.");
}