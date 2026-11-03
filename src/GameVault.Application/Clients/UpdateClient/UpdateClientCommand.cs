namespace GameVault.Application.Clients.UpdateClient;

public sealed record UpdateClientCommand(Guid Id, string Name, string Email);