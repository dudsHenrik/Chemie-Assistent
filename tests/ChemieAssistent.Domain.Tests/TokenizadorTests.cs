using ChemieAssistent.Domain;

public class TokenizadorTests
{
    [Fact]
    public void CaOH2_ViraSeisTokens()
    {
        var tokens = Tokenizador.Ler("Ca(OH)2");

        Assert.Equal(6, tokens.Count);
        Assert.Equal(TipoToken.Simbolo, tokens[0].Tipo);
        Assert.Equal("Ca", tokens[0].Texto);
    }

    [Fact]
    public void H2O_ViraQuatroTokens()
    {
        var tokens = Tokenizador.Ler("2H2O");

        Assert.Equal(4, tokens.Count);
        Assert.Equal(TipoToken.Coeficiente, tokens[0].Tipo);
        Assert.Equal("2", tokens[0].Texto);
    }

    [Fact]
    public void CuSO4_5H20_ViraNoveTokens()
    {
        var tokens = Tokenizador.Ler("CuSO4*5H2O");
        Assert.Equal(9, tokens.Count);
        Assert.Equal(TipoToken.Simbolo, tokens[0].Tipo);
        Assert.Equal("Cu", tokens[0].Texto);
    }

    [Fact]
    public void CuSO4_H20_ViraOitoTokens()
    {
        var tokens = Tokenizador.Ler("CuSO4*H2O");
        Assert.Equal(8, tokens.Count);
        Assert.Equal(TipoToken.Simbolo, tokens[0].Tipo);
        Assert.Equal("Cu", tokens[0].Texto);
    }
}