using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace GameVault.Api.Endpoints;

public static class EndpointExtensions
{
    /// <summary>
    /// Escanea el ensamblado proporcionado y registra todos los tipos que implementan IEndpoint en el contenedor de DI.
    /// </summary>
    public static IServiceCollection AddEndpoints(this IServiceCollection services, Assembly assembly)
    {
        var endpointDescriptors = assembly
            .DefinedTypes
            .Where(type => type is { IsAbstract: false, IsInterface: false } &&
                           type.IsAssignableTo(typeof(IEndpoint)))
            .Select(type => ServiceDescriptor.Transient(typeof(IEndpoint), type))
            .ToArray();

        services.TryAddEnumerable(endpointDescriptors);

        return services;
    }

    /// <summary>
    /// Resuelve todos los IEndpoint y los mapea bajo un prefijo raíz común (por defecto '/api').
    /// </summary>
    /// <param name="app">La aplicación web.</param>
    /// <param name="prefix">El prefijo base global para todos los endpoints (ej. "/api" o "/api/v1").</param>
    public static IApplicationBuilder MapEndpoints(this WebApplication app, string? prefix = "/api")
    {
        var endpoints = app.Services.GetRequiredService<IEnumerable<IEndpoint>>();

        // Si se define prefijo, se crea el RouteGroup centralizado; de lo contrario, se usa app directamente
        IEndpointRouteBuilder routeBuilder = string.IsNullOrWhiteSpace(prefix)
            ? app
            : app.MapGroup(prefix);

        foreach (var endpoint in endpoints)
        {
            endpoint.MapEndpoint(routeBuilder);
        }

        return app;
    }
}
