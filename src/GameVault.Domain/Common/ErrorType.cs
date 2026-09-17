namespace GameVault.Domain.Common;

/// <summary>
/// Categorización de los errores de negocio para su correcto mapeo a estados HTTP.
/// </summary>
public enum ErrorType
{
    Failure = 0,
    Validation = 1,
    NotFound = 2,
    Conflict = 3,
    Unauthorized = 4,
    Forbidden = 5
}
