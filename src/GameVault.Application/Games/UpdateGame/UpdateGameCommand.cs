namespace GameVault.Application.Games.UpdateGame;

public sealed record UpdateGameCommand(Guid Id, string Title, string Genre, decimal Price);