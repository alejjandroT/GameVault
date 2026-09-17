using GameVault.Api.Endpoints;
using GameVault.Domain.Common;

namespace GameVault.Domain.Tests;

public class UnitTest1
{
    [Fact]
    public void Endpoint_ShouldUseGameVaultNaming()
    {
        var endpoints = typeof(JuegosEndpoints);

        Assert.Equal("JuegosEndpoints", endpoints.Name);
        Assert.NotNull(typeof(Result));
        Assert.NotNull(typeof(Error));
    }
}
