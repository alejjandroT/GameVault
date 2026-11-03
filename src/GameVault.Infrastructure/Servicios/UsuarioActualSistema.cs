using GameVault.Application.Abstractions;

namespace GameVault.Infrastructure.Servicios;

internal sealed class UsuarioActualSistema : IUsuarioActual
{
    public string Nombre => "sistema";
}