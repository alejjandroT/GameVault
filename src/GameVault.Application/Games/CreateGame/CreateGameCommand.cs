namespace GameVault.Application.Games.CreateGame;

public sealed record CreateGameCommand(string Title, string Genre, decimal Price);
