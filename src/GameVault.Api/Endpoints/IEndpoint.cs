namespace GameVault.Api.Endpoints;

/// <summary>
/// Contrato base para la definición y registro modular de endpoints en Minimal APIs.
/// </summary>
public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}
