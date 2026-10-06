namespace ChemieAssistent.Domain;

public sealed record EquacaoTexto(List<string> Reagentes, List<string> Produtos, bool Reversivel);

public static class LeitorEquacao
{
    public static EquacaoTexto Ler(string equacao)
    {
        if (string.IsNullOrWhiteSpace(equacao))
            throw new DominioException(CodigoErro.FormulaVazia, "A equação não pode ser vazia.");

        string setaNormalizada = equacao.Replace("<->", "⇌").Replace("->", "→");

        int setas = 0;
        for (int i = 0; i < setaNormalizada.Length; i++)
        {
            if (setaNormalizada[i] == '→' || setaNormalizada[i] == '⇌')
            {
                setas++;
            }
        }

        if (setas == 0)
            throw new DominioException(CodigoErro.SetaAusente, "Falta a seta da reação.");
        if (setas > 1)
            throw new DominioException(CodigoErro.SetaDuplicada, "Só pode haver uma seta.");
        

        int posSeta = setaNormalizada.IndexOfAny(new[] { '→', '⇌' });
        bool reversivel = setaNormalizada[posSeta] == '⇌';

        string ladoEsquerdo = setaNormalizada[..posSeta];
        string ladoDireito = setaNormalizada[(posSeta + 1)..];

        return new EquacaoTexto(
            CortarLado(ladoEsquerdo, CodigoErro.ReagenteAusente, "reagentes"),
            CortarLado(ladoDireito, CodigoErro.ProdutoAusente, "produtos"),
            reversivel
        );
    }

    private static List<string> CortarLado(string lado, CodigoErro codigoVazio, string nome)
    {
        var compostos = new List<string>();
        string[] pedacos = lado.Split('+');

        for (int i = 0; i < pedacos.Length; i++)
        {
            string composto = pedacos[i].Trim();
            if (composto.Length == 0)
                throw new DominioException(codigoVazio,
                    $"Há um composto vazio nos {nome}.");
            compostos.Add(composto);
        }

        return compostos;
    }
}