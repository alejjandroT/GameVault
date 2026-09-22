namespace GameVault.Api.Endpoints;


/// Contrato base para la definición y registro modular de endpoints en Minimal APIs.
public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}
