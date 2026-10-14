using ChemieAssistent.Domain;
using static ChemieAssistent.Domain.Estequiometria;

namespace ChemieAssistent.Domain.Tests;

public class ProcessadorEquacaoTests
{
    private readonly ProcessadorEquacao _processador
        = new(IRepositorioElementosFake.Padrao());

    [Fact]
    [Trait("Categoria", "Fluxo")]
    public void Proporcao_sai_dos_coeficientes_balanceados()
    {
        var r = _processador.Processar("C3H8 + O2 -> CO2 + H2O");

        Assert.Equal("1 : 5 : 3 : 4", r.ProporcaoTexto);
        Assert.Equal(SituacaoCoeficientes.NaoInformados, r.Situacao);
        Assert.Null(r.Quantidades);
        Assert.Null(r.Limitante);
    }

    [Fact]
    [Trait("Categoria", "Fluxo")]
    public void Uma_massa_gera_quantidades()
    {
        var r = _processador.Processar("C3H8 + O2 -> CO2 + H2O",
            new[] { new EntradaMassa(0, 88.194m, UnidadeMassa.Grama) });

        Assert.NotNull(r.Quantidades);
        Assert.Equal(319.98m, r.Quantidades![1].MassaGramas, precision: 3);
    }

    [Fact]
    [Trait("Categoria", "Fluxo")]
    public void Duas_massas_geram_limitante()
    {
        var r = _processador.Processar("C3H8 + O2 -> CO2 + H2O", new[]
        {
            new EntradaMassa(0, 88.194m, UnidadeMassa.Grama),
            new EntradaMassa(1, 159.99m, UnidadeMassa.Grama),
        });

        Assert.NotNull(r.Limitante);
        Assert.Equal(1, r.Limitante!.IndiceLimitante);
    }

    [Fact]
    [Trait("Categoria", "Fluxo")]
    public void Coeficiente_errado_nao_impede_o_calculo()
    {
        var r = _processador.Processar("C3H8 + 3O2 -> 3CO2 + 4H2O",
            new[] { new EntradaMassa(0, 88.194m, UnidadeMassa.Grama) });

        Assert.Equal(SituacaoCoeficientes.Incorretos, r.Situacao);
        Assert.Single(r.Divergencias);
        Assert.Equal(319.98m, r.Quantidades![1].MassaGramas, precision: 3);
    }

    [Fact]
    [Trait("Categoria", "Validacao")]
    public void Elemento_inexistente_falha_antes_de_balancear()
    {
        var ex = Assert.Throws<DominioException>(
            () => _processador.Processar("Xx2O -> Xx + O2"));

        Assert.Equal(CodigoErro.ElementoInexistente, ex.Codigo);
    }

    [Fact]
    [Trait("Categoria", "Validacao")]
    public void Texto_acima_do_limite_e_rejeitado()
    {
        var ex = Assert.Throws<DominioException>(
            () => _processador.Processar(new string('H', ProcessadorEquacao.LimiteCaracteres + 1)));

        Assert.Equal(CodigoErro.EntradaMuitoLonga, ex.Codigo);
    }

    [Fact]
    [Trait("Categoria", "Validacao")]
    public void Overflow_de_inteiro_na_formula_vira_erro_de_dominio()
    {
        // exige <CheckForOverflowUnderflow> no csproj do Domain
        var ex = Assert.Throws<DominioException>(
            () => _processador.Processar("Ca(OH2147483647)2147483647 -> Ca"));

        Assert.Equal(CodigoErro.CalculoMuitoGrande, ex.Codigo);
    }

    [Fact]
    [Trait("Categoria", "Validacao")]
    public void Overflow_de_decimal_na_massa_vira_erro_de_dominio()
    {
        var ex = Assert.Throws<DominioException>(() =>
            _processador.Processar("C3H8 + O2 -> CO2 + H2O",
                new[] { new EntradaMassa(0, decimal.MaxValue, UnidadeMassa.Quilograma) }));

        Assert.Equal(CodigoErro.CalculoMuitoGrande, ex.Codigo);
    }
}