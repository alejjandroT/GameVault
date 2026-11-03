using System.Data;
using GameVault.Application.Abstractions;
using GameVault.Infrastructure.Persistence;

namespace GameVault.Infrastructure.Persistence.Repositories;

internal abstract class RepositoryBase(
    IDbConnectionFactory connectionFactory,
    TimeProvider reloj,
    IUsuarioActual usuario)
{
    protected IDbConnectionFactory ConnectionFactory { get; } = connectionFactory;
    protected DateTime NowUtc => reloj.GetUtcNow().UtcDateTime;
    protected string CurrentUser => usuario.Nombre;
}