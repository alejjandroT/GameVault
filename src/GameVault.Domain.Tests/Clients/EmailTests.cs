using GameVault.Domain.Clients;

namespace GameVault.Domain.Tests.Clients;

public sealed class EmailTests
{
    [Theory]
    [InlineData("sin-arroba", "Client.EmailInvalid")]
    [InlineData("usuario@", "Client.EmailInvalid")]
    [InlineData("@ejemplo.com", "Client.EmailInvalid")]
    [InlineData("   ", "Client.EmailRequired")]
    public void Create_ConFormatoInvalido_DebeFallar(string value, string expectedCode)
    {
        var result = Email.Create(value);

        Assert.True(result.IsFailure);
        Assert.Equal(expectedCode, result.Error.Code);
    }

    [Fact]
    public void Create_ConMayusculasYEspacios_DebeNormalizarAMinusculas()
    {
        var result = Email.Create("  Ana.Garcia@Example.COM  ");

        Assert.True(result.IsSuccess);
        Assert.Equal("ana.garcia@example.com", result.Value.Value);
    }
}