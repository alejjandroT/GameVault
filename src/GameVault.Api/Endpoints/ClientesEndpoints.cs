using GameVault.Api.Contracts;
using GameVault.Api.Extensions;
using GameVault.Application.Clients.CreateClient;
using GameVault.Application.Clients.DeactivateClient;
using GameVault.Application.Clients.GetClientById;
using GameVault.Application.Clients.ListClients;
using GameVault.Application.Clients.UpdateClient;
using GameVault.Domain.Clients;
using GameVault.Domain.Common;

namespace GameVault.Api.Endpoints;

public sealed class ClientesEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/clientes")
                       .WithTags("Clientes");

        group.MapGet("/", async (ListClientsHandler handler, CancellationToken ct) =>
        {
            var clients = await handler.HandleAsync(new ListClientsCommand(), ct);
            var response = clients.Select(ToResponse).ToList();
            return Result.Success(response).ToHttpResult();
        })
        .WithName("ObtenerClientes")
        .WithSummary("Obtiene la lista de clientes");

        group.MapGet("/{id:guid}", async (Guid id, GetClientByIdHandler handler, CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(new GetClientByIdCommand(new ClientId(id)), ct);
            return result.IsSuccess
                ? Result.Success(ToResponse(result.Value)).ToHttpResult()
                : Result.Failure<ClienteResponse>(result.Error).ToHttpResult();
        })
        .WithName("ObtenerClientePorId")
        .WithSummary("Obtiene el detalle de un cliente por su identificador GUID");

        group.MapPost("/", async (CrearClienteRequest request, CreateClientHandler handler, CancellationToken ct) =>
        {
            var command = new CreateClientCommand(request.Nombre, request.Email);
            var result = await handler.HandleAsync(command, ct);
            return result.IsSuccess
                ? Result.Success(ToResponse(result.Value)).ToHttpCreatedAtResult($"/api/clientes/{result.Value.Id.Value}")
                : Result.Failure<ClienteResponse>(result.Error).ToHttpResult();
        })
        .WithName("CrearCliente")
        .WithSummary("Registra un cliente nuevo");

        group.MapPut("/{id:guid}", async (Guid id, ActualizarClienteRequest request, UpdateClientHandler handler, CancellationToken ct) =>
        {
            var command = new UpdateClientCommand(id, request.Nombre, request.Email);
            var result = await handler.HandleAsync(command, ct);
            return result.IsSuccess
                ? Result.Success().ToHttpResult()
                : Result.Failure<ClienteResponse>(result.Error).ToHttpResult();
        })
        .WithName("ActualizarCliente")
        .WithSummary("Actualiza los datos de un cliente existente");

        group.MapDelete("/{id:guid}", async (Guid id, DeactivateClientHandler handler, CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(new DeactivateClientCommand(id), ct);
            return result.IsSuccess
                ? Result.Success().ToHttpResult()
                : Result.Failure(result.Error).ToHttpResult();
        })
        .WithName("DesactivarCliente")
        .WithSummary("Desactiva lógicamente un cliente");
    }

    private static ClienteResponse ToResponse(Client client) => new(
        Id: client.Id.Value,
        Nombre: client.Name,
        Email: client.Email.Value,
        Activo: client.Active,
        CreadoEn: client.CreatedAt,
        CreadoPor: client.CreatedBy,
        ActualizadoEn: client.UpdatedAt,
        ActualizadoPor: client.UpdatedBy);
}