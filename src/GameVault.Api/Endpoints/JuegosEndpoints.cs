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
            Nombre: "Dayz Gone",
            Categoria: "Aventura",
            NumeroLicencia: "Dayz-12345",
            Email: "sofia.ramirez@gamevault.com",
            Activo: true
        ),
        new(
            Id: 2,
            Nombre: "The Last of Us",
            Categoria: "Aventura",
            NumeroLicencia: "Last-67890",
            Email: "andres.castro@gamevault.com",
            Activo: true
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
        if (string.IsNullOrWhiteSpace(request.Nombre) || string.IsNullOrWhiteSpace(request.NumeroLicencia))
        {
            Result<JuegoResponse> validationResult = Error.Validation(
                "Juego.Validacion",
                "El nombre y el número de licencia son campos obligatorios.");

            return validationResult.ToHttpResult();
        }

        if (JuegosDb.Any(j => j.NumeroLicencia.Equals(request.NumeroLicencia, StringComparison.OrdinalIgnoreCase)))
        {
            Result<JuegoResponse> conflictResult = Error.Conflict(
                "Juego.LicenciaDuplicada",
                $"Ya existe un juego registrado con el número de licencia '{request.NumeroLicencia}'.");

            return conflictResult.ToHttpResult();
        }

        var nuevoId = JuegosDb.Count != 0 ? JuegosDb.Max(j => j.Id) + 1 : 1;

        var nuevoJuego = new JuegoResponse(
            Id: nuevoId,
            Nombre: request.Nombre.Trim(),
            Categoria: request.Categoria.Trim(),
            NumeroLicencia: request.NumeroLicencia.Trim(),
            Email: request.Email.Trim(),
            Activo: true
        );

        JuegosDb.Add(nuevoJuego);

        Result<JuegoResponse> createdResult = Result.Success(nuevoJuego);
        return createdResult.ToHttpCreatedAtResult($"/api/juegos/{nuevoJuego.Id}");
    }
}
