using ChemieAssistent.Domain;
using static ChemieAssistent.Domain.Estequiometria;

namespace ChemieAssistent.Api;

// ---------- Entrada ----------
public sealed record EntradaMassaDto(int IndiceComposto, decimal Valor, string Unidade);
public sealed record CalcularRequest(string? Equacao, List<EntradaMassaDto>? Massas);

// ---------- Saída ----------
public sealed record CompostoDto(int Indice, string Formula, int Coeficiente, bool EhReagente, decimal MassaMolar);
public sealed record DivergenciaDto(int Indice, string Composto, int Digitado, int Esperado);
public sealed record QuantidadeDto(
    int Indice, string Formula, bool EhReagente, decimal MassaMolar,
    decimal Mols, decimal MassaGramas,
    decimal? ExcedenteMols, decimal? ExcedenteGramas);
public sealed record LimitanteDto(int IndiceLimitante, List<QuantidadeDto> Linhas);

public sealed record RelatorioDto(
    string EquacaoBalanceada,
    bool Reversivel,
    SituacaoCoeficientes Situacao,
    List<DivergenciaDto> Divergencias,
    string Proporcao,
    List<CompostoDto> Compostos,
    List<QuantidadeDto>? Quantidades,
    LimitanteDto? Limitante);

public static class Mapeamento
{
    public static EntradaMassa ParaDominio(this EntradaMassaDto dto)
    {
        UnidadeMassa unidade = dto.Unidade?.Trim().ToLowerInvariant() switch
        {
            "g" => UnidadeMassa.Grama,
            "kg" => UnidadeMassa.Quilograma,
            _ => throw new DominioException(CodigoErro.EntradaInvalida,
                    $"Unidade '{dto.Unidade}' inválida. Use 'g' ou 'kg'.")
        };
        return new EntradaMassa(dto.IndiceComposto, dto.Valor, unidade);
    }

    public static RelatorioDto ParaDto(this RelatorioEstequiometrico r)
    {
        var todos = r.Equacao.Reagentes.Concat(r.Equacao.Produtos).ToList();
        int qtdReagentes = r.Equacao.Reagentes.Count;

        var compostos = todos.Select((c, i) =>
            new CompostoDto(i, Formula(c), c.Coeficiente, i < qtdReagentes, r.MassasMolares[i])).ToList();

        var quantidades = r.Quantidades?.Select((l, i) =>
            new QuantidadeDto(i, Formula(l.Composto), l.EhReagente, l.MassaMolar,
                              l.Mols, l.MassaGramas, null, null)).ToList();

        LimitanteDto? limitante = r.Limitante is null ? null : new LimitanteDto(
            r.Limitante.IndiceLimitante,
            r.Limitante.Linhas.Select((l, i) =>
                new QuantidadeDto(i, Formula(l.Composto), l.EhReagente, l.MassaMolar,
                                  l.Mols, l.MassaGramas, l.ExcedenteMols, l.ExcedenteGramas)).ToList());

        string seta = r.Equacao.Reversivel ? "⇌" : "→";
        string equacao = $"{Lado(r.Equacao.Reagentes)} {seta} {Lado(r.Equacao.Produtos)}";

        return new RelatorioDto(
            equacao, r.Equacao.Reversivel, r.Situacao,
            r.Divergencias.Select(d => new DivergenciaDto(d.Indice, d.Composto, d.Digitado, d.Esperado)).ToList(),
            r.ProporcaoTexto, compostos, quantidades, limitante);
    }

    // Texto guarda o que foi digitado (com coeficiente); a fórmula é o resto.
    private static string Formula(Composto c) => c.Texto.TrimStart("0123456789".ToCharArray());

    private static string Lado(IEnumerable<Composto> compostos)
        => string.Join(" + ", compostos.Select(c => (c.Coeficiente > 1 ? c.Coeficiente.ToString() : "") + Formula(c)));
}