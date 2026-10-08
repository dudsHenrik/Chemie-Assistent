using ChemieAssistent.Domain;

public class BalanceadorTests
{
    private static int[] Coeficientes(string texto)
    {
        var eq = Balanceador.Balancear(InterpretadorEquacao.Interpretar(texto));
        return eq.Reagentes.Concat(eq.Produtos).Select(c => c.Coeficiente).ToArray();
    }

    [Theory]
    [InlineData("C3H8 + O2 -> CO2 + H2O", new[] { 1, 5, 3, 4 })]
    [InlineData("H2 + O2 -> H2O", new[] { 2, 1, 2 })]
    [InlineData("Fe + O2 -> Fe2O3", new[] { 4, 3, 2 })]
    [InlineData("CuSO4*5H2O -> CuSO4 + H2O", new[] { 1, 1, 5 })]
    [InlineData("[Cu(H2O)4]SO4*H2O +  NH3 -> H2O + [Cu(NH3)4]SO4*2H2O", new[] { 1, 4, 3, 1 })]
    public void Balanceia(string texto, int[] esperado)
        => Assert.Equal(esperado, Coeficientes(texto));

    [Theory]
    [InlineData("H2 -> O2", CodigoErro.ElementosDivergentes)]
    [InlineData("H2O -> H2O2", CodigoErro.SemSolucao)]
    [InlineData("H2 + O2 -> H2O + H2O2", CodigoErro.EquacaoIndeterminada)]
    public void LancaErro(string texto, CodigoErro esperado)
    {
        var ex = Assert.Throws<DominioException>(() => Coeficientes(texto));
        Assert.Equal(esperado, ex.Codigo);
    }
}