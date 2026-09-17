using GameVault.Api.Contracts;
using GameVault.Api.Extensions;
using GameVault.Domain.Common;

namespace GameVault.Api.Endpoints;

public class JuegosEndpoints : IEndpoint
{
    private static readonly List<JuegoResponse> JuegosDb =
    [
        new(
            Id: 1,
            Titulo: "The Legend of Zelda: Breath of the Wild",
            Genero: "Aventura",
            Precio: 59.99m,
            Publicado: true
        ),
        new(
            Id: 2,
            Titulo: "Cyberpunk 2077",
            Genero: "RPG",
            Precio: 49.99m,
            Publicado: true
        ),
        new(
            Id: 3,
            Titulo: "Hades",
            Genero: "Indie",
            Precio: 24.99m,
            Publicado: true
        )
    ];

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/juegos")
                       .WithTags("Juegos");

        group.MapGet("/", ObtenerTodos)
             .WithName("ObtenerJuegos")
             .WithSummary("Obtiene la lista de juegos disponibles");

        group.MapGet("/{id:int}", ObtenerPorId)
             .WithName("ObtenerJuegoPorId")
             .WithSummary("Obtiene el detalle de un juego por su identificador entero usando Result<T>");

        group.MapPost("/", CrearJuego)
             .WithName("CrearJuego")
             .WithSummary("Registra un nuevo juego validando reglas con Result<T>");
    }

    private static IResult ObtenerTodos()
    {
        var result = Result.Success(JuegosDb.AsReadOnly());
        return result.ToHttpResult();
    }

    private static IResult ObtenerPorId(int id)
    {
        var juego = JuegosDb.FirstOrDefault(j => j.Id == id);
        if (juego is null)
        {
            Result<JuegoResponse> errorResult = Error.NotFound(
                "Juego.NotFound",
                $"No se encontró un juego con el Id: {id}");

            return errorResult.ToHttpResult();
        }

        Result<JuegoResponse> successResult = Result.Success(juego);
        return successResult.ToHttpResult();
    }

    private static IResult CrearJuego(CrearJuegoRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Titulo) || string.IsNullOrWhiteSpace(request.Genero) || request.Precio <= 0)
        {
            Result<JuegoResponse> validationResult = Error.Validation(
                "Juego.Validacion",
                "El título, el género y el precio son campos obligatorios.");

            return validationResult.ToHttpResult();
        }

        if (JuegosDb.Any(j => j.Titulo.Equals(request.Titulo, StringComparison.OrdinalIgnoreCase)))
        {
            Result<JuegoResponse> conflictResult = Error.Conflict(
                "Juego.TituloDuplicado",
                $"Ya existe un juego registrado con el título '{request.Titulo}'.");

            return conflictResult.ToHttpResult();
        }

        var nuevoId = JuegosDb.Count != 0 ? JuegosDb.Max(j => j.Id) + 1 : 1;

        var nuevoJuego = new JuegoResponse(
            Id: nuevoId,
            Titulo: request.Titulo.Trim(),
            Genero: request.Genero.Trim(),
            Precio: request.Precio,
            Publicado: true
        );

        JuegosDb.Add(nuevoJuego);

        Result<JuegoResponse> createdResult = Result.Success(nuevoJuego);
        return createdResult.ToHttpCreatedAtResult($"/api/juegos/{nuevoJuego.Id}");
    }
}
