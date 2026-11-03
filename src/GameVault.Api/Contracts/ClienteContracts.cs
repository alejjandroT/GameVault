namespace GameVault.Api.Contracts;

public record ClienteResponse(
    Guid Id,
    string Nombre,
    string Email,
    bool Activo,
    DateTime CreadoEn,
    string CreadoPor,
    DateTime? ActualizadoEn,
    string? ActualizadoPor);

public record CrearClienteRequest(string Nombre, string Email);

public record ActualizarClienteRequest(string Nombre, string Email);