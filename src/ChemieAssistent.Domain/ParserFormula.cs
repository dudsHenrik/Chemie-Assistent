namespace ChemieAssistent.Domain;

public static class ParserFormula
{
    public static Composto Ler(string formula)
    {
        if (string.IsNullOrWhiteSpace(formula))
            throw new DominioException(CodigoErro.FormulaVazia, "Fórmula vazia.");

        string texto = formula.Trim();
        List<Token> tokens = Tokenizador.Ler(texto);
        return Interpretar(texto, tokens);
    }

    public static Composto Interpretar(string texto, List<Token> tokens)
    {
        int coeficiente = 1;
        int coeficienteHidrato = 1;

        var principal = new Dictionary<string, int>();
        var atual = principal;
        var pilha = new Stack<(Dictionary<string, int> Atomos, TipoToken Abertura)>();
        bool emHidrato = false;

        for (int i = 0; i < tokens.Count; i++)
        {
            Token t = tokens[i];

            switch (t.Tipo)
            {
                case TipoToken.Coeficiente:
                    coeficiente = ValidarQuantidade(t);
                    break;

                case TipoToken.Simbolo:
                    {
                        int quantidade = 1;

                        if (i + 1 < tokens.Count && tokens[i + 1].Tipo == TipoToken.IndiceInterno)
                        {
                            quantidade = ValidarQuantidade(tokens[i + 1]);
                            i++; // consumiu o índice
                        }

                        Somar(atual, t.Texto, quantidade);
                        break;
                    }

                case TipoToken.AbreParenteses:
                case TipoToken.AbreColchete:
                    pilha.Push((atual, t.Tipo));
                    atual = new Dictionary<string, int>();
                    break;

                case TipoToken.FechaParenteses:
                case TipoToken.FechaColchete:
                    {
                        if (pilha.Count == 0)
                            throw new DominioException(CodigoErro.ParentesesDesbalanceados, "Fechamento sem abertura.");

                        var grupo = atual;
                        (atual, TipoToken abertura) = pilha.Pop();

                        bool combina = (t.Tipo == TipoToken.FechaParenteses && abertura == TipoToken.AbreParenteses)
                                    || (t.Tipo == TipoToken.FechaColchete && abertura == TipoToken.AbreColchete);

                        if (!combina)
                            throw new DominioException(CodigoErro.ParentesesDesbalanceados, "Parênteses e colchetes cruzados.");

                        int indice = 1;

                        if (i + 1 < tokens.Count && tokens[i + 1].Tipo == TipoToken.IndiceExterno)
                        {
                            indice = ValidarQuantidade(tokens[i + 1]);
                            i++; // consumiu o índice
                        }
                        else if (t.Tipo == TipoToken.FechaParenteses)
                        {
                            throw new DominioException(CodigoErro.ParentesesDesbalanceados, "Parênteses sem índice.");
                        }

                        Mesclar(atual, grupo, indice);
                        break;
                    }

                case TipoToken.Hidrato:
                    {
                        if (pilha.Count > 0 || emHidrato)
                            throw new DominioException(CodigoErro.ParentesesDesbalanceados, "Hidrato em posição inválida.");

                        if (i + 1 < tokens.Count && tokens[i + 1].Tipo == TipoToken.CoeficienteHidrato)
                        {
                            coeficienteHidrato = ValidarQuantidade(tokens[i + 1]);
                            i++;
                        }

                        emHidrato = true;
                        atual = new Dictionary<string, int>(); // átomos da água de hidratação
                        break;
                    }

                default:
                    throw new DominioException(CodigoErro.FormulaVazia, $"Token inesperado: {t.Texto}");
            }
        }

        if (pilha.Count > 0)
            throw new DominioException(CodigoErro.ParentesesDesbalanceados, "Parênteses ou colchetes não fechados.");

        if (emHidrato)
            Mesclar(principal, atual, coeficienteHidrato);

        // Opção A: átomos por unidade de fórmula; o coeficiente fica separado
        return new Composto(texto, coeficiente, coeficienteHidrato, principal);
    }

    private static int ValidarQuantidade(Token t)
    {
        string valor = t.Texto;

        if (valor.StartsWith('0'))
            throw new DominioException(CodigoErro.QuantidadeInvalida, $"Quantidade inválida: {valor}");

        if (valor == "1")
            throw new DominioException(CodigoErro.QuantidadeInvalida, $"Quantidade {valor} é implícita e deve ser omitida");

        return int.TryParse(valor, out int result)
            ? result
            : throw new DominioException(CodigoErro.QuantidadeInvalida, $"Quantidade inválida: {valor}");
    }

    private static void Somar(Dictionary<string, int> d, string simbolo, int quantidade)
    {
        d[simbolo] = d.GetValueOrDefault(simbolo) + quantidade;
    }

    private static void Mesclar(Dictionary<string, int> destino, Dictionary<string, int> origem, int fator)
    {
        foreach (var (simbolo, quantidade) in origem)
            Somar(destino, simbolo, quantidade * fator);
    }
}