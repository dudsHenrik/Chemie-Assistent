using ChemieAssistent.Domain;
using static Estequiometria;

namespace ChemieAssistent.Domain.Tests;

public class EstequiometriaTests
{
    private readonly Estequiometria _estequiometria;

    public EstequiometriaTests()
    {
        var massaMolar = new CalculadoraMassaMolar(IRepositorioElementosFake.Padrao());
        _estequiometria = new Estequiometria(massaMolar);
    }

    private static Equacao Balanceada(string texto)
        => Balanceador.Balancear(InterpretadorEquacao.Interpretar(texto));

    private static decimal SomaMassas(IEnumerable<LinhaResultado> linhas)
        => linhas.Sum(l => l.MassaGramas);

    [Fact]
    public void Propano_88_194g_gera_mols_e_massas_esperados()
    {
        var eq = Balanceada("C3H8 + O2 -> CO2 + H2O");

        var linhas = _estequiometria.Calcular(
            eq, new EntradaMassa(0, 88.194m, UnidadeMassa.Grama));

        decimal[] molsEsperados = { 2m, 10m, 6m, 8m };
        decimal[] massasEsperadas = { 88.194m, 319.98m, 264.054m, 144.12m };

        Assert.Equal(4, linhas.Count);

        for (int i = 0; i < linhas.Count; i++)
        {
            Assert.Equal(molsEsperados[i], linhas[i].Mols, precision: 6);
            Assert.Equal(massasEsperadas[i], linhas[i].MassaGramas, precision: 3);
        }

        Assert.True(linhas[0].EhReagente);
        Assert.False(linhas[2].EhReagente);
    }

    [Fact]
    public void Massa_se_conserva_entre_reagentes_e_produtos()
    {
        var eq = Balanceada("C3H8 + O2 -> CO2 + H2O");

        var linhas = _estequiometria.Calcular(
            eq, new EntradaMassa(0, 88.194m, UnidadeMassa.Grama));

        decimal reagentes = SomaMassas(linhas.Where(l => l.EhReagente));
        decimal produtos = SomaMassas(linhas.Where(l => !l.EhReagente));

        Assert.Equal(reagentes, produtos, precision: 3);
    }

    [Fact]
    public void Um_quilo_e_mil_gramas_dao_o_mesmo_resultado()
    {
        var eq = Balanceada("Fe + O2 -> Fe2O3");

        var emKg = _estequiometria.Calcular(eq, new EntradaMassa(0, 1m, UnidadeMassa.Quilograma));
        var emG = _estequiometria.Calcular(eq, new EntradaMassa(0, 1000m, UnidadeMassa.Grama));

        for (int i = 0; i < emG.Count; i++)
            Assert.Equal(emG[i].MassaGramas, emKg[i].MassaGramas);
    }

    [Fact]
    public void Um_quilo_de_ferro_gera_as_massas_esperadas()
    {
        var eq = Balanceada("Fe + O2 -> Fe2O3");

        var linhas = _estequiometria.Calcular(
            eq, new EntradaMassa(0, 1m, UnidadeMassa.Quilograma));

        Assert.Equal(429.73m, linhas[1].MassaGramas, precision: 2);   // O2
        Assert.Equal(1429.73m, linhas[2].MassaGramas, precision: 2);   // Fe2O3
    }

    [Fact]
    public void Composto_informado_pode_ser_um_produto()
    {
        var eq = Balanceada("C3H8 + O2 -> CO2 + H2O");

        // 264,054 g de CO2 (índice 2) devem corresponder a 88,194 g de C3H8
        var linhas = _estequiometria.Calcular(
            eq, new EntradaMassa(2, 264.054m, UnidadeMassa.Grama));

        Assert.Equal(88.194m, linhas[0].MassaGramas, precision: 3);
    }

    [Fact]
    public void Hidrato_gera_agua_na_proporcao_correta()
    {
        var eq = Balanceada("CuSO4*5H2O -> CuSO4 + H2O");

        var linhas = _estequiometria.Calcular(
            eq, new EntradaMassa(0, 249.677m, UnidadeMassa.Grama));

        Assert.Equal(1m, linhas[0].Mols, precision: 6);
        Assert.Equal(159.602m, linhas[1].MassaGramas, precision: 3);   // CuSO4
        Assert.Equal(5m, linhas[2].Mols, precision: 6);   // H2O
        Assert.Equal(90.075m, linhas[2].MassaGramas, precision: 3);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Massa_nao_positiva_lanca_QuantidadeInvalida(double valor)
    {
        var eq = Balanceada("C3H8 + O2 -> CO2 + H2O");

        var ex = Assert.Throws<DominioException>(() =>
            _estequiometria.Calcular(eq, new EntradaMassa(0, (decimal)valor, UnidadeMassa.Grama)));

        Assert.Equal(CodigoErro.QuantidadeInvalida, ex.Codigo);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(4)]
    [InlineData(9)]
    public void Indice_fora_da_lista_lanca_CompostoAusente(int indice)
    {
        var eq = Balanceada("C3H8 + O2 -> CO2 + H2O");   // índices válidos: 0 a 3

        var ex = Assert.Throws<DominioException>(() =>
            _estequiometria.Calcular(eq, new EntradaMassa(indice, 10m, UnidadeMassa.Grama)));

        Assert.Equal(CodigoErro.CompostoAusente, ex.Codigo);
    }
}