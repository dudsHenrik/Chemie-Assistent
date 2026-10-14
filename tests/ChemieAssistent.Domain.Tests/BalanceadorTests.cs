using ChemieAssistent.Domain;

namespace ChemieAssistent.Domain.Tests;

public class BalanceadorTests
{
    private static int[] Coeficientes(string texto)
    {
        var eq = Balanceador.Balancear(InterpretadorEquacao.Interpretar(texto));
        return eq.Reagentes.Concat(eq.Produtos).Select(c => c.Coeficiente).ToArray();
    }

    [Theory]
    [Trait("Categoria", "Balanceamento")]
    [InlineData("C3H8 + O2 -> CO2 + H2O", new[] { 1, 5, 3, 4 })]
    [InlineData("H2 + O2 -> H2O", new[] { 2, 1, 2 })]
    [InlineData("Fe + O2 -> Fe2O3", new[] { 4, 3, 2 })]
    [InlineData("CuSO4*5H2O -> CuSO4 + H2O", new[] { 1, 1, 5 })]
    [InlineData("[Cu(H2O)4]SO4*H2O +  NH3 -> H2O + [Cu(NH3)4]SO4*2H2O", new[] { 1, 4, 3, 1 })]
    public void Balanceia(string texto, int[] esperado)
        => Assert.Equal(esperado, Coeficientes(texto));

    [Theory]
    [Trait("Categoria", "Validacao")]
    [InlineData("H2 -> O2", CodigoErro.ElementosDivergentes)]
    [InlineData("H2O -> H2O2", CodigoErro.SemSolucao)]
    [InlineData("H2 + O2 -> H2O + H2O2", CodigoErro.EquacaoIndeterminada)]
    public void LancaErro(string texto, CodigoErro esperado)
    {
        var ex = Assert.Throws<DominioException>(() => Coeficientes(texto));
        Assert.Equal(esperado, ex.Codigo);
    }

    [Theory]
    [Trait("Categoria", "Validacao")]
    [InlineData("C3H8 + O2 -> CO2 + H2O", SituacaoCoeficientes.NaoInformados)]
    [InlineData("C3H8 + 5O2 -> 3CO2 + 4H2O", SituacaoCoeficientes.Corretos)]
    [InlineData("2C3H8 + 10O2 -> 6CO2 + 8H2O", SituacaoCoeficientes.Proporcionais)]
    [InlineData("C3H8 + 3O2 -> 3CO2 + 4H2O", SituacaoCoeficientes.Incorretos)]
    public void Classifica_os_coeficientes_digitados(string texto, SituacaoCoeficientes esperado)
    {
        var r = Balanceador.BalancearEVerificar(InterpretadorEquacao.Interpretar(texto));
        Assert.Equal(esperado, r.Situacao);
    }

    [Fact]
    [Trait("Categoria", "Validacao")]
    public void Reporta_so_o_composto_divergente()
    {
        var r = Balanceador.BalancearEVerificar(
            InterpretadorEquacao.Interpretar("C3H8 + 3O2 -> 3CO2 + 4H2O"));

        var d = Assert.Single(r.Divergencias);
        Assert.Equal(("O2", 3, 5), (d.Composto, d.Digitado, d.Esperado));
    }

    [Fact]
    [Trait("Categoria", "Validacao")]
    public void Equacao_devolvida_tem_os_coeficientes_balanceados()
    {
        var r = Balanceador.BalancearEVerificar(
            InterpretadorEquacao.Interpretar("C3H8 + 3O2 -> 3CO2 + 4H2O"));

        Assert.Equal(5, r.Equacao.Reagentes[1].Coeficiente);
    }
}