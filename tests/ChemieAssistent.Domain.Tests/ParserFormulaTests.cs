using ChemieAssistent.Domain;

public class ParserFormulaTests
{
    [Fact]
    public void Fe2O3()
        => Assert.Equal(new Dictionary<string, int> { ["Fe"] = 2, ["O"] = 3 },
                        ParserFormula.Ler("Fe2O3"));

    [Fact]
    public void Parenteses()
        => Assert.Equal(new Dictionary<string, int> { ["Ca"] = 1, ["O"] = 2, ["H"] = 2 },
                        ParserFormula.Ler("Ca(OH)2"));

    [Fact]
    public void ParentesesAninhados()
        => Assert.Equal(new Dictionary<string, int> { ["Al"] = 2, ["S"] = 3, ["O"] = 12 },
                        ParserFormula.Ler("Al2(SO4)3"));

    [Fact]
    public void MaiusculaMinuscula_CO_eh_diferente_de_Co()
    {
        Assert.Equal(new Dictionary<string, int> { ["C"] = 1, ["O"] = 1 }, ParserFormula.Ler("CO"));
        Assert.Equal(new Dictionary<string, int> { ["Co"] = 1 }, ParserFormula.Ler("Co"));
    }

    [Fact]
    public void ParentesesAbertos_LancaErro()
    {
        var ex = Assert.Throws<DominioException>(() => ParserFormula.Ler("Ca(OH"));
        Assert.Equal(CodigoErro.ParentesesDesbalanceados, ex.Codigo);
    }

    [Fact]
    public void QuantidadeZero_LancaErro()
    {
        var ex = Assert.Throws<DominioException>(() => ParserFormula.Ler("Fe0O3"));
        Assert.Equal(CodigoErro.CompostoInvalido, ex.Codigo);
    }
    [Fact]
    public void QuantidadeUm_LancaErro()
    {
        var ex = Assert.Throws<DominioException>(() => ParserFormula.Ler("Fe1O3"));
        Assert.Equal(CodigoErro.CompostoInvalido, ex.Codigo);

    }
}