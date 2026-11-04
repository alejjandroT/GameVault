namespace GameVault.Api.Contracts;

public record PagarOrdenRequest(string Metodo, string Token);

public record PagoResponse(
    Guid Id,
    Guid OrdenId,
    decimal Monto,
    string Metodo,
    string Estado,
    DateTime CreadoEn,
    string CreadoPor);

public record LicenciaResponse(
    Guid Id,
    Guid OrdenId,
    Guid JuegoId,
    string Clave,
    DateTime EmitidaEn);