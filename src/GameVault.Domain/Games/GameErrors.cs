// GameVault.Domain/Games/GameErrors.cs
using GameVault.Domain.Common;

namespace GameVault.Domain.Games;

public static class GameErrors
{
    public static readonly Error EmptyTitle = Error.Validation(
        "Game.EmptyTitle", "El título del juego no puede estar vacío.");

    public static readonly Error TitleTooLong = Error.Validation(
        "Game.TitleTooLong", "El título no puede superar los 100 caracteres.");

    public static readonly Error NegativePrice = Error.Validation(
        "Game.NegativePrice", "El precio no puede ser negativo.");

    public static readonly Error PriceExceedsMax = Error.Validation(
        "Game.PriceExceedsMax", "El precio no puede superar los $500.");

    public static readonly Error CannotPublish = Error.Conflict(
        "Game.CannotPublish", "Solo un juego en estado Draft puede publicarse.");

    public static readonly Error CannotDiscontinue = Error.Conflict(
        "Game.CannotDiscontinue", "Solo un juego Publicado puede descontinuarse.");

    public static Error NotFound(GameId id) => Error.NotFound(
        "Game.NotFound", $"No se encontró un juego con el Id '{id.Value}'.");

    public static Error DuplicateTitle(string title) => Error.Conflict(
        "Game.DuplicateTitle", $"Ya existe un juego registrado con el título '{title}'.");

    public static Error DuplicateTitle(string title) => Error.Conflict(
        "Game.DuplicateTitle", $"Ya existe un juego registrado con el título '{title}'.");
        
}