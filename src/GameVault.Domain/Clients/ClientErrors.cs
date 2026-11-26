using GameVault.Domain.Common;

namespace GameVault.Domain.Clients;

public static class ClientErrors
{
    public static readonly Error InvalidName = Error.Validation(
        "Client.InvalidName", "El nombre del cliente debe tener entre 2 y 100 caracteres.");

    public static readonly Error EmailRequired = Error.Validation(
        "Client.EmailRequired", "El correo electrónico del cliente es obligatorio.");

    public static readonly Error EmailInvalid = Error.Validation(
        "Client.EmailInvalid", "El formato del correo electrónico no es válido.");

    public static readonly Error AlreadyInactive = Error.Conflict(
        "Client.AlreadyInactive", "El cliente ya está inactivo.");

    public static readonly Error AlreadyActive = Error.Conflict(
        "Client.AlreadyActive", "El cliente ya está activo.");

    public static Error NotFound(ClientId id) => Error.NotFound(
        "Client.NotFound", $"No se encontró un cliente con el Id '{id.Value}'.");

    public static Error DuplicateEmail(string email) => Error.Conflict(
        "Client.DuplicateEmail", $"Ya existe un cliente registrado con el correo '{email}'.");
}