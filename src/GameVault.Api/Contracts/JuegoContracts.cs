namespace GameVault.Api.Contracts;

public record JuegoResponse(
    int Id,
    string Nombre,
    string Categoria,
    string NumeroLicencia,
    string Email,
    bool Activo);

public record CrearJuegoRequest(
    string Nombre,
    string Categoria,
    string NumeroLicencia,
    string Email);
