using GameVault.Domain.Clients;

namespace GameVault.Application.Clients.GetClientById;

public sealed record GetClientByIdCommand(ClientId Id);