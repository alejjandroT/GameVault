namespace GameVault.Api.Contracts;

public record JuegoResponse(
    Guid Id,
    string Titulo,
    string Genero,
    decimal Precio,
    string Estado,
    DateTime CreadoEn,
    string CreadoPor,
    DateTime? ActualizadoEn,
    string? ActualizadoPor);

public record CrearJuegoRequest(
    string Titulo,
    string Genero,
    decimal Precio);

public record ActualizarJuegoRequest(
    string Titulo,
    string Genero,
    decimal Precio);
