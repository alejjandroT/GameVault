using GameVault.Api.Contracts;
using GameVault.Api.Extensions;
using GameVault.Application.Orders.CancelOrder;
using GameVault.Application.Orders.CreateOrder;
using GameVault.Application.Orders.GetOrderById;
using GameVault.Application.Orders.ListOrders;
using GameVault.Application.Payments.PayOrder;
using GameVault.Domain.Common;
using GameVault.Domain.Games;
using GameVault.Domain.Clients;
using GameVault.Domain.Orders;
using GameVault.Domain.Payments;

namespace GameVault.Api.Endpoints;

public sealed class OrdenesEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/ordenes")
                       .WithTags("Ordenes");

        group.MapPost("/", async (CrearOrdenRequest request, CreateOrderHandler handler, CancellationToken ct) =>
        {
            var command = new CreateOrderCommand(
                new ClientId(request.ClienteId),
                request.JuegoIds.Select(id => new GameId(id)).ToList());
            var result = await handler.HandleAsync(command, ct);
            return result.IsSuccess
                ? Result.Success(ToResponse(result.Value)).ToHttpCreatedAtResult($"/api/ordenes/{result.Value.Id.Value}")
                : Result.Failure<OrdenResponse>(result.Error).ToHttpResult();
        })
        .WithName("CrearOrden")
        .WithSummary("Crea una orden para un cliente con juegos publicados");

        group.MapGet("/", async (ListOrdersHandler handler, CancellationToken ct) =>
        {
            var orders = await handler.HandleAsync(new ListOrdersCommand(), ct);
            var response = orders.Select(ToResponse).ToList();
            return Result.Success(response).ToHttpResult();
        })
        .WithName("ObtenerOrdenes")
        .WithSummary("Obtiene la lista de órdenes");

        group.MapGet("/{id:guid}", async (Guid id, GetOrderByIdHandler handler, CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(new GetOrderByIdCommand(new OrderId(id)), ct);
            return result.IsSuccess
                ? Result.Success(ToResponse(result.Value)).ToHttpResult()
                : Result.Failure<OrdenResponse>(result.Error).ToHttpResult();
        })
        .WithName("ObtenerOrdenPorId")
        .WithSummary("Obtiene el detalle de una orden por su identificador GUID");

        group.MapPost("/{id:guid}/cancelar", async (Guid id, CancelOrderHandler handler, CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(new CancelOrderCommand(new OrderId(id)), ct);
            return result.IsSuccess
                ? Result.Success().ToHttpResult()
                : Result.Failure(result.Error).ToHttpResult();
        })
        .WithName("CancelarOrden")
        .WithSummary("Cancela una orden pendiente");

        group.MapPost("/{id:guid}/pagar", async (Guid id, PagarOrdenRequest request, PayOrderHandler handler, CancellationToken ct) =>
        {
            if (!Enum.TryParse<PaymentMethod>(request.Metodo, ignoreCase: true, out var method)
                || !Enum.IsDefined(method)
                || !Enum.GetNames<PaymentMethod>().Contains(request.Metodo, StringComparer.OrdinalIgnoreCase))
            {
                var invalidMethod = Error.Validation(
                    "Payment.InvalidMethod", "El método de pago debe ser Card o Wallet.");
                return Result.Failure<PagoResponse>(invalidMethod).ToHttpResult();
            }

            var command = new PayOrderCommand(new OrderId(id), method, request.Token);
            var result = await handler.HandleAsync(command, ct);
            return result.IsSuccess
                ? Result.Success(ToResponse(result.Value)).ToHttpCreatedAtResult($"/api/ordenes/{id}")
                : Result.Failure<PagoResponse>(result.Error).ToHttpResult();
        })
        .WithName("PagarOrden")
        .WithSummary("Procesa el pago de una orden pendiente");
    }

    private static OrdenResponse ToResponse(Order order) => new(
        Id: order.Id.Value,
        ClienteId: order.ClientId.Value,
        Estado: order.Status.ToString(),
        Items: order.Items
            .Select(item => new OrdenItemResponse(item.GameId.Value, item.UnitPrice.Amount))
            .ToList(),
        Total: order.Total,
        CreadoEn: order.CreatedAt,
        CreadoPor: order.CreatedBy,
        ActualizadoEn: order.UpdatedAt,
        ActualizadoPor: order.UpdatedBy);

    private static PagoResponse ToResponse(Payment payment) => new(
        Id: payment.Id.Value,
        OrdenId: payment.OrderId.Value,
        Monto: payment.Amount,
        Metodo: payment.Method.ToString(),
        Estado: payment.Status.ToString(),
        CreadoEn: payment.CreatedAt,
        CreadoPor: payment.CreatedBy);
}