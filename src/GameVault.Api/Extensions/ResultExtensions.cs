using GameVault.Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace GameVault.Api.Extensions;

/// <summary>
/// Métodos de extensión para mapear instancias de Result y Result<T> a IResult de Minimal APIs (RFC 7807).
/// </summary>
public static class ResultExtensions
{
    public static IResult ToHttpResult<TValue>(this Result<TValue> result)
    {
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.ToProblemDetailsResult();
    }

    public static IResult ToHttpCreatedAtResult<TValue>(this Result<TValue> result, string uri)
    {
        return result.IsSuccess
            ? Results.Created(uri, result.Value)
            : result.ToProblemDetailsResult();
    }

    public static IResult ToHttpResult(this Result result)
    {
        return result.IsSuccess
            ? Results.NoContent()
            : result.ToProblemDetailsResult();
    }

    private static IResult ToProblemDetailsResult(this Result result)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException("No se puede convertir un resultado exitoso a ProblemDetails de error.");
        }

        var error = result.Error;

        var (statusCode, title, type) = error.Type switch
        {
            ErrorType.Validation => (
                StatusCodes.Status400BadRequest,
                "Error de Validación",
                "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1"
            ),
            ErrorType.NotFound => (
                StatusCodes.Status404NotFound,
                "Recurso No Encontrado",
                "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4"
            ),
            ErrorType.Conflict => (
                StatusCodes.Status409Conflict,
                "Conflicto de Estado",
                "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.8"
            ),
            ErrorType.Unauthorized => (
                StatusCodes.Status401Unauthorized,
                "No Autorizado",
                "https://datatracker.ietf.org/doc/html/rfc7235#section-3.1"
            ),
            ErrorType.Forbidden => (
                StatusCodes.Status403Forbidden,
                "Acceso Prohibido",
                "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.3"
            ),
            _ => (
                StatusCodes.Status500InternalServerError,
                "Fallo en la Operación",
                "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1"
            )
        };

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = error.Description,
            Type = type
        };

        problemDetails.Extensions["errorCode"] = error.Code;

        return Results.Problem(problemDetails);
    }
}
