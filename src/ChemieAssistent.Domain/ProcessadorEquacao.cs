using static ChemieAssistent.Domain.Estequiometria;

namespace ChemieAssistent.Domain;

// Reúne os dados do resultado estequiométrico, incluindo o balanceamento,
// a proporção dos coeficientes, as massas molares e os cálculos solicitados.
public sealed record RelatorioEstequiometrico(
    Equacao Equacao,
    SituacaoCoeficientes Situacao,
    IReadOnlyList<Divergencia> Divergencias,
    IReadOnlyList<int> Proporcao,             // Reagentes e produtos na ordem da equação.
    IReadOnlyList<decimal> MassasMolares,     // Na mesma ordem da proporção.
    List<LinhaResultado>? Quantidades,        // Resultado quando uma massa é informada.
    ResultadoLimitante? Limitante)            // Resultado quando duas ou mais massas são informadas.
{
    public string ProporcaoTexto => string.Join(" : ", Proporcao);
}

// Coordena a interpretação, o balanceamento e os cálculos estequiométricos
// para produzir um relatório a partir do texto de uma equação química.
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