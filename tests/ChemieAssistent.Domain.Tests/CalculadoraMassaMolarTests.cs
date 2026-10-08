using ChemieAssistent.Domain;

namespace ChemieAssistent.Domain.Tests;

public class CalculadoraMassaMolarTests
{
    private readonly CalculadoraMassaMolar _calculadora
        = new(IRepositorioElementosFake.Padrao());

    private decimal MassaMolarDe(string formula)
        => _calculadora.Calcular(ParserFormula.Ler(formula));

    [Theory]
    [InlineData("H2O", 18.015)]
    [InlineData("O2", 31.998)]
    [InlineData("CO2", 44.009)]
    [InlineData("C3H8", 44.097)]
    [InlineData("Fe2O3", 159.687)]
    [InlineData("Ca(OH)2", 74.092)]
    [InlineData("CuSO4*5H2O", 249.677)]
    public void Calcula_a_massa_molar(string formula, double esperado)
        => Assert.Equal((decimal)esperado, MassaMolarDe(formula));

    [Fact]
    public void Coeficiente_do_composto_nao_altera_a_massa_molar()
    {
        Assert.Equal(MassaMolarDe("CuSO4*5H2O"), MassaMolarDe("2CuSO4*5H2O"));
    }

    [Fact]
    public void Arredonda_a_quatro_casas_com_AwayFromZero()
    {
        // 1,23445 -> 1,2345 (AwayFromZero). O arredondamento bancário daria 1,2344.
        var repositorio = new IRepositorioElementosFake(
            new Elemento("Zz", "Teste", 1.23445m));
        var calculadora = new CalculadoraMassaMolar(repositorio);

        decimal mm = calculadora.Calcular(ParserFormula.Ler("Zz"));

        Assert.Equal(1.2345m, mm);
    }

    [Fact]
    public void Elemento_inexistente_lanca_ElementoInexistente()
    {
        var ex = Assert.Throws<DominioException>(() => MassaMolarDe("Xx2O"));

        Assert.Equal(CodigoErro.ElementoInexistente, ex.Codigo);
    }
}