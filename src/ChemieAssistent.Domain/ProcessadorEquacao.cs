using static ChemieAssistent.Domain.Estequiometria;

namespace ChemieAssistent.Domain;

public sealed record RelatorioEstequiometrico(
    Equacao Equacao,                          // já com os coeficientes balanceados
    SituacaoCoeficientes Situacao,
    IReadOnlyList<Divergencia> Divergencias,
    IReadOnlyList<int> Proporcao,             // reagentes + produtos, na ordem da equação
    IReadOnlyList<decimal> MassasMolares,     // mesma ordem da Proporcao
    List<LinhaResultado>? Quantidades,        // preenchido com 1 massa informada
    ResultadoLimitante? Limitante)            // preenchido com 2 ou mais massas
{
    public string ProporcaoTexto => string.Join(" : ", Proporcao);
}

public sealed class ProcessadorEquacao
{
    public const int LimiteCaracteres = 500;

    private readonly CalculadoraMassaMolar _massaMolar;
    private readonly Estequiometria _estequiometria;

    public ProcessadorEquacao(IRepositorioElementos repositorio)
    {
        _massaMolar = new CalculadoraMassaMolar(repositorio);
        _estequiometria = new Estequiometria(_massaMolar);
    }

    public RelatorioEstequiometrico Processar(
        string texto,
        IReadOnlyList<EntradaMassa>? entradas = null)
    {
        ValidarTamanho(texto);

        try
        {
            return Executar(texto, entradas ?? Array.Empty<EntradaMassa>());
        }
        catch (OverflowException)
        {
            throw new DominioException(CodigoErro.CalculoMuitoGrande,
                "Os números dessa equação são grandes demais para serem calculados.");
        }
    }

    private RelatorioEstequiometrico Executar(string texto, IReadOnlyList<EntradaMassa> entradas)
    {
        Equacao lida = InterpretadorEquacao.Interpretar(texto);

        var massasMolares = lida.Reagentes.Concat(lida.Produtos)
            .Select(_massaMolar.Calcular)
            .ToList();

        ResultadoBalanceamento balanceamento = Balanceador.BalancearEVerificar(lida);
        Equacao eq = balanceamento.Equacao;

        var proporcao = eq.Reagentes.Concat(eq.Produtos)
            .Select(c => c.Coeficiente)
            .ToList();

        List<LinhaResultado>? quantidades = null;
        ResultadoLimitante? limitante = null;

        if (entradas.Count == 1)
            quantidades = _estequiometria.Calcular(eq, entradas[0]);
        else if (entradas.Count >= 2)
            limitante = _estequiometria.CalcularLimitante(eq, entradas);

        return new RelatorioEstequiometrico(
            eq, balanceamento.Situacao, balanceamento.Divergencias,
            proporcao, massasMolares, quantidades, limitante);
    }

    private static void ValidarTamanho(string texto)
    {
        if (texto is { Length: > LimiteCaracteres })
            throw new DominioException(CodigoErro.EntradaMuitoLonga,
                $"A equação pode ter no máximo {LimiteCaracteres} caracteres.");
    }
}