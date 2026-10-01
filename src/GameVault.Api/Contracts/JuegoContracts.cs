namespace GameVault.Api.Contracts;

public record JuegoResponse(
    Guid Id,
    string Titulo,
    string Genero,
    decimal Precio,
    string Estado);

public record CrearJuegoRequest(
    string Titulo,
    string Genero,
    decimal Precio);
