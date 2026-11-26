using GameVault.Domain.Clients;

namespace GameVault.Domain.Tests.Clients;

public sealed class ClientTests
{
    [Fact]
    public void Create_ConDatosValidos_DebeCrearClienteActivo()
    {
        var result = Client.Create("Ana García", "ANA@example.com");

        Assert.True(result.IsSuccess);
        Assert.Equal("Ana García", result.Value.Name);
        Assert.Equal("ana@example.com", result.Value.Email.Value);
        Assert.True(result.Value.Active);
    }

    [Fact]
    public void Deactivate_DosVeces_DebeFallarLaSegundaVez()
    {
        var client = Client.Create("Ana García", "ana@example.com").Value;

        Assert.True(client.Deactivate().IsSuccess);
        var secondResult = client.Deactivate();

        Assert.True(secondResult.IsFailure);
        Assert.Equal(ClientErrors.AlreadyInactive, secondResult.Error);
    }
}