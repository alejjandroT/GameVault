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

public record LicenciaDetalleResponse(
    Guid Id,
    string Clave,
    DateTime EmitidaEn,
    JuegoResumenResponse Juego);

public record JuegoResumenResponse(Guid Id, string Titulo, string Genero);