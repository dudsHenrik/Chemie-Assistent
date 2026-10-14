using ChemieAssistent.Domain;

namespace ChemieAssistent.Domain.Tests;

public class ParserFormulaTests
{
    [Theory]
    [Trait("Categoria", "Interpretar")]
    [InlineData("Fe2O3", new string[] { "Fe", "O" }, new int[] { 2, 3 })]
    [InlineData("H2O", new string[] { "H", "O" }, new int[] { 2, 1 })]
    [InlineData("CO2", new string[] { "C", "O" }, new int[] { 1, 2 })]
    public void InterpretarFormulaSimples(
        string formula,
        string[] simbolosEsperados,
        int[] quantidadesEsperadas)
    {
        var composto = ParserFormula.Ler(formula);

        Assert.Equal(
            new Dictionary<string, int>
            {
                [simbolosEsperados[0]] = quantidadesEsperadas[0],
                [simbolosEsperados[1]] = quantidadesEsperadas[1]
            },
            composto.Atomos
        );
    }

    [Theory]
    [Trait("Categoria", "Interpretar")]
    [InlineData("[Cu(NH3)4]SO4*H2O", 1, new string[] { "Cu", "N", "H", "S", "O"}, new int[] { 1, 4, 14, 1, 5})]
    [InlineData("K4[Fe(CN)6]*12H2O", 1, new string[] { "K", "Fe", "C", "N", "H", "O" }, new int[] { 4, 1, 6, 6, 24, 12 })]
    [InlineData("2K4[Fe(CN)6]*12H2O", 2, new string[] { "K", "Fe", "C", "N", "H", "O" }, new int[] { 4, 1, 6, 6, 24, 12 })] //Parser não calcula o coeficiente do composto
    public void InterpretarFormulaComplexa(
        string formula,
        int coeficienteEsperado,
        string[] simbolosEsperados,
        int[] quantidadesEsperadas)
    {
        var composto = ParserFormula.Ler(formula);

        var atomosEsperados = simbolosEsperados
            .Zip(quantidadesEsperadas)
            .ToDictionary(x => x.First, x => x.Second);

        Assert.Equal(coeficienteEsperado, composto.Coeficiente);
        Assert.Equal(atomosEsperados, composto.Atomos);
    }

    [Fact]
    [Trait("Categoria", "Capitalizacao")]
    public void DiferenciarMaiusculasEMinusculas()
    {
        var co = ParserFormula.Ler("CO");
        var Co = ParserFormula.Ler("Co");

        Assert.Equal(
            new Dictionary<string, int>
            {
                ["C"] = 1,
                ["O"] = 1
            },
            co.Atomos
        );

        Assert.Equal(
            new Dictionary<string, int>
            {
                ["Co"] = 1
            },
            Co.Atomos
        );
    }

    [Fact]
    [Trait("Categoria", "Coeficiente")]
    public void SepararCoeficienteDosAtomos()
    {
        var composto = ParserFormula.Ler("2H2O");

        Assert.Equal(2, composto.Coeficiente);

        Assert.Equal(
            new Dictionary<string, int>
            {
                ["H"] = 2,
                ["O"] = 1
            },
            composto.Atomos
        );
    }

    [Theory]
    [Trait("Categoria", "Validacao")]
    [InlineData("Fe0O3", CodigoErro.QuantidadeInvalida)]
    [InlineData("Fe1O3", CodigoErro.QuantidadeInvalida)]
    public void RejeitarFormulaInvalida(
        string formula,
        CodigoErro codigoEsperado)
    {
        var ex = Assert.Throws<DominioException>(
            () => ParserFormula.Ler(formula)
        );

        Assert.Equal(codigoEsperado, ex.Codigo);
    }

    [Fact]
    [Trait("Categoria", "Parenteses")]
    public void InterpretarFormulaComParenteses()
    {
        var composto = ParserFormula.Ler("Ca(OH)2");

        Assert.Equal(
            new Dictionary<string, int>
            {
                ["Ca"] = 1,
                ["O"] = 2,
                ["H"] = 2
            },
            composto.Atomos
        );
    }

    [Fact]
    [Trait("Categoria", "Parenteses")]
    public void InterpretarParentesesAninhados()
    {
        var composto = ParserFormula.Ler("Al2(SO4)3");

        Assert.Equal(
            new Dictionary<string, int>
            {
                ["Al"] = 2,
                ["S"] = 3,
                ["O"] = 12
            },
            composto.Atomos
        );
    }

    [Fact]
    [Trait("Categoria", "Parenteses")]
    public void RejeitarParentesesDesbalanceados()
    {
        var ex = Assert.Throws<DominioException>(
            () => ParserFormula.Ler("Ca(OH")
        );

        Assert.Equal(
            CodigoErro.ParentesesDesbalanceados,
            ex.Codigo
        );
    }
}