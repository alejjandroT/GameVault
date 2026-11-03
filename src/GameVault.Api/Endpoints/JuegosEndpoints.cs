using GameVault.Api.Contracts;
using GameVault.Api.Extensions;
using GameVault.Application.Games.CreateGame;
using GameVault.Application.Games.GetGameById;
using GameVault.Application.Games.ListGames;
using GameVault.Application.Games.UpdateGame;
using GameVault.Application.Games.PublishGame;
using GameVault.Application.Games.DiscontinueGame;
using GameVault.Domain.Common;
using GameVault.Domain.Games;

namespace GameVault.Api.Endpoints;

public class JuegosEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/juegos")
                       .WithTags("Juegos");

        group.MapGet("/", async (ListGamesHandler handler, CancellationToken ct) =>
        {
            var games = await handler.HandleAsync(ct);
            var response = games.Select(ToResponse).ToList();
            return Result.Success(response).ToHttpResult();
        })
        .WithName("ObtenerJuegos")
        .WithSummary("Obtiene la lista de juegos disponibles");

        group.MapGet("/{id:guid}", async (Guid id, GetGameByIdHandler handler, CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(new GameId(id), ct);
            if (result.IsSuccess)
            {
                return Result.Success(ToResponse(result.Value)).ToHttpResult();
            }

            return Result.Failure<JuegoResponse>(result.Error).ToHttpResult();
        })
        .WithName("ObtenerJuegoPorId")
        .WithSummary("Obtiene el detalle de un juego por su identificador GUID usando Result<T>");

        group.MapPost("/", async (CrearJuegoRequest request, CreateGameHandler handler, CancellationToken ct) =>
        {
            var command = new CreateGameCommand(request.Titulo, request.Genero, request.Precio);
            var result = await handler.HandleAsync(command, ct);

            if (result.IsSuccess)
            {
                return Result.Success(ToResponse(result.Value)).ToHttpCreatedAtResult($"/api/juegos/{result.Value.Id.Value}");
            }

            return Result.Failure<JuegoResponse>(result.Error).ToHttpResult();
        })
        .WithName("CrearJuego")
        .WithSummary("Registra un nuevo juego validando reglas con Result<T>");

        group.MapPut("/{id:guid}", async (Guid id, ActualizarJuegoRequest request, UpdateGameHandler handler, CancellationToken ct) =>
        {
            var command = new UpdateGameCommand(id, request.Titulo, request.Genero, request.Precio);
            var result = await handler.HandleAsync(command, ct);

            return result.IsSuccess
                ? Result.Success().ToHttpResult()
                : Result.Failure<JuegoResponse>(result.Error).ToHttpResult();
        })
        .WithName("ActualizarJuego")
        .WithSummary("Actualiza los datos de un juego existente");

        group.MapDelete("/{id:guid}", async (Guid id, DiscontinueGameHandler handler, CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(new GameId(id), ct);
            return result.IsSuccess
                ? Result.Success().ToHttpResult()
                : Result.Failure(result.Error).ToHttpResult();
        })
        .WithName("DescontinuarJuego")
        .WithSummary("Descontinúa lógicamente un juego publicado");

        group.MapPost("/{id:guid}/publicar", async (Guid id, PublishGameHandler handler, CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(new GameId(id), ct);
            return result.IsSuccess
                ? Result.Success().ToHttpResult()
                : Result.Failure<JuegoResponse>(result.Error).ToHttpResult();
        })
        .WithName("PublicarJuego")
        .WithSummary("Publica un juego en borrador");
    }

    private static JuegoResponse ToResponse(Game game)
    {
        return new JuegoResponse(
            Id: game.Id.Value,
            Titulo: game.Title,
            Genero: game.Genre,
            Precio: game.Price.Amount,
            Estado: game.Status.ToString(),
            CreadoEn: game.CreatedAt,
            CreadoPor: game.CreatedBy,
            ActualizadoEn: game.UpdatedAt,
            ActualizadoPor: game.UpdatedBy);
    }
}
