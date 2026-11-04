using GameVault.Domain.Common;

namespace GameVault.Domain.Orders;

public static class OrderErrors
{
    public static Error GameNotAvailable(string title) => Error.Conflict(
        "Order.GameNotAvailable", $"El juego '{title}' no está disponible para compra.");

    public static Error DuplicateGame(string title) => Error.Conflict(
        "Order.DuplicateGame", $"El juego '{title}' ya fue agregado a la orden.");

    public static readonly Error EmptyOrder = Error.Validation(
        "Order.EmptyOrder", "La orden debe contener al menos un juego.");

    public static readonly Error ClientInactive = Error.Conflict(
        "Order.ClientInactive", "No se puede crear una orden para un cliente inactivo.");

    public static readonly Error NotModifiable = Error.Conflict(
        "Order.NotModifiable", "Solo se pueden modificar órdenes pendientes.");

    public static readonly Error CannotCancel = Error.Conflict(
        "Order.CannotCancel", "Solo se pueden cancelar órdenes pendientes.");

    public static readonly Error CannotPay = Error.Conflict(
        "Order.CannotPay", "Solo se pueden pagar órdenes pendientes que contengan al menos un juego.");

    public static Error NotFound(OrderId id) => Error.NotFound(
        "Order.NotFound", $"No se encontró una orden con el Id '{id.Value}'.");
}