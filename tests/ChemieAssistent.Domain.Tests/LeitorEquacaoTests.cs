using ChemieAssistent.Domain;

public class LeitorEquacaoTests
{
    [Fact]
    public void EquacaoSimples()
    {
        var eq = LeitorEquacao.Ler("2H2 + O2 -> 2H2O");

        Assert.Equal(new List<string> { "2H2", "O2" }, eq.Reagentes);
        Assert.Equal(new List<string> { "2H2O" }, eq.Produtos);
        Assert.False(eq.Reversivel);
    }

    [Theory]
    [InlineData("H2 + O2", CodigoErro.SetaAusente)]
    [InlineData("H2 + O2 H2O", CodigoErro.SetaAusente)]
    [InlineData("H2 -> O2 -> H2O", CodigoErro.SetaDuplicada)]
    [InlineData("H2 + + O2 -> H2O", CodigoErro.ReagenteAusente)]
    [InlineData("H2 + O2 -> H2O + ", CodigoErro.ProdutoAusente)]
    [InlineData("-> H2O", CodigoErro.ReagenteAusente)]
    [InlineData("H2 + O2 ->", CodigoErro.ProdutoAusente)]
    public void EntradasInvalidas(string entrada, CodigoErro esperado)
    {
        var ex = Assert.Throws<DominioException>(() => LeitorEquacao.Ler(entrada));
        Assert.Equal(esperado, ex.Codigo);
    }
}