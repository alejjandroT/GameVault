namespace GameVault.Api.Contracts;

public record OrdenItemResponse(Guid JuegoId, decimal PrecioUnitario);

public record OrdenResponse(
    Guid Id,
    Guid ClienteId,
    string Estado,
    IReadOnlyList<OrdenItemResponse> Items,
    decimal Total,
    DateTime CreadoEn,
    string CreadoPor,
    DateTime? ActualizadoEn,
    string? ActualizadoPor);

public record CrearOrdenRequest(Guid ClienteId, IReadOnlyList<Guid> JuegoIds);