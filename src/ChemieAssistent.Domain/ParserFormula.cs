namespace ChemieAssistent.Domain;

public static class ParserFormula
{
    public static IReadOnlyDictionary<string, int> Ler(string formula)
    {
        if (string.IsNullOrWhiteSpace(formula))
            throw new DominioException(CodigoErro.FormulaVazia, "Fórmula vazia.");

        int pos = 0;
        return LerGrupo(formula.Trim(), ref pos, aninhado: false);
    }

    // Lê átomos até o fim da fórmula ou até o ')' que fecha o grupo atual.
    private static Dictionary<string, int> LerGrupo(string f, ref int pos, bool aninhado)
    {
        var contagem = new Dictionary<string, int>();
        int mol = 1;

        while (pos < f.Length)
        {
            char c = f[pos];
            if (char.IsDigit(c))
            {
                mol = LerNumero(f, ref pos);
                pos++;
            }
            else if (c == '(')
            {
                pos++;
                var interno = LerGrupo(f, ref pos, aninhado: true); // consome até o ')'
                int multiplicador = LerNumero(f, ref pos);
                foreach (var (simbolo, qtd) in interno)
                    Somar(contagem, simbolo, qtd * multiplicador);
                if (interno == null)
                    throw new DominioException(CodigoErro.FormulaVazia, 
                        "Grupo inválido.");
            }
            else if (c == ')')
            {
                if (!aninhado)
                    throw new DominioException(CodigoErro.ParentesesDesbalanceados,
                        "Parêntese fechado sem abertura.");
                if (LerGrupo(f, ref pos, aninhado: false) == null)
                    throw new DominioException(CodigoErro.ParentesesDesbalanceados, 
                        "Grupo inválido.");
                pos++;
                return contagem;
            }
            else if (char.IsUpper(c))
            {
                int inicio = pos++;
                if (pos < f.Length && char.IsLower(f[pos])) pos++;   // símbolo: 1 maiúscula + 0 ou 1 minúscula
                string simbolo = f[inicio..pos];
                Somar(contagem, simbolo, LerNumero(f, ref pos));
            }
            else
            {
                throw new DominioException(CodigoErro.CaractereInvalido,
                    $"Caractere inválido '{c}' na posição {pos + 1}.");
            }
        }

        if (aninhado)
            throw new DominioException(CodigoErro.ParentesesDesbalanceados,
                "Parêntese aberto sem fechamento.");

        return contagem;
    }

    // Lê dígitos consecutivos; se não houver, o valor é 1 (ex.: "O" = 1 átomo).
    private static int LerNumero(string f, ref int pos)
    {
        int inicio = pos;
        while (pos < f.Length && char.IsDigit(f[pos])) pos++;
        if (pos == inicio) return 1;

        string texto = f[inicio..pos];
        if (texto == "0")
            throw new DominioException(CodigoErro.CompostoInvalido,
                   $"Quantidade inválida '0' na posição {pos}.");
        if (texto == "1")
            throw new DominioException(CodigoErro.CompostoInvalido,
                $"Quantidade inválida '1' na posição {pos}.");

        return int.Parse(texto);
    }

    private static void Somar(Dictionary<string, int> d, string simbolo, int qtd)
        => d[simbolo] = d.GetValueOrDefault(simbolo) + qtd;
}