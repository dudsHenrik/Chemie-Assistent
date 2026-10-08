using ChemieAssistent.Domain;

public class InterpretadorEquacaoTests
{
    [Theory]
    [InlineData("2H2 + O2 -> 2H2O", 2, 2, 1, 1, false)]
    [InlineData("4Fe + 3O2 -> 2Fe2O3", 2, 4, 3, 3, false)]
    public void LeEquacaoCompleta(
        string texto,
        int quantidadeReagentes,
        int coeficientePrimeiroReagente,
        int coeficienteSegundoReagente,
        int quantidadeOxigeniosProduto,
        bool reversivel)
    {
        var eq = InterpretadorEquacao.Interpretar(texto);

        Assert.Equal(quantidadeReagentes, eq.Reagentes.Count);
        Assert.Equal(coeficientePrimeiroReagente, eq.Reagentes[0].Coeficiente);
        Assert.Equal(coeficienteSegundoReagente, eq.Reagentes[1].Coeficiente);
        Assert.Equal(quantidadeOxigeniosProduto, eq.Produtos[0].Atomos["O"]);
        Assert.Equal(reversivel, eq.Reversivel);
    }

    [Theory]
    [InlineData("2N2 + 3H2 <-> 2NH3", true)]
    [InlineData("NH4Cl <-> NH3 + HCl", true)]
    [InlineData("CO2 + H2O <-> H2CO3", true)]
    public void LeEquacaoReversivel(
        string texto,
        bool reversivelEsperado)
    {
        var eq = InterpretadorEquacao.Interpretar(texto);

        Assert.Equal(reversivelEsperado, eq.Reversivel);
    }
}