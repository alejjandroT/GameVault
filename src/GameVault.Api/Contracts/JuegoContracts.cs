namespace GameVault.Api.Contracts;

public record JuegoResponse(
    int Id,
    string Titulo,
    string Genero,
    decimal Precio,
    bool Publicado);

public record CrearJuegoRequest(
    string Titulo,
    string Genero,
    decimal Precio);
