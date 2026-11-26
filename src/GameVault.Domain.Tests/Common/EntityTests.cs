using GameVault.Domain.Common;

namespace GameVault.Domain.Tests.Common;

public sealed class EntityTests
{
    [Fact]
    public void Equals_ConMismoTipoYMismoId_DebeSerIgual()
    {
        Entity<int> first = new FirstEntity(7);
        Entity<int> second = new FirstEntity(7);

        Assert.Equal(first, second);
        Assert.True(first == second);
    }

    [Fact]
    public void Equals_ConMismoTipoYDistintoId_DebeSerDistinto()
    {
        Entity<int> first = new FirstEntity(7);
        Entity<int> second = new FirstEntity(8);

        Assert.NotEqual(first, second);
    }

    [Fact]
    public void Equals_ConTiposDistintosYMismoId_DebeSerDistinto()
    {
        Entity<int> first = new FirstEntity(7);
        Entity<int> second = new SecondEntity(7);

        Assert.NotEqual(first, second);
    }

    private sealed class FirstEntity(int id) : Entity<int>(id);

    private sealed class SecondEntity(int id) : Entity<int>(id);
}