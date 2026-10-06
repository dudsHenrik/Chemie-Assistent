namespace ChemieAssistent.Domain;

public static class Tokenizador
{
    public static List<Token> Ler(string f)
    {
        var tokens = new List<Token>();
        for (int pos = 0; pos < f.Length;)
        {
            char c = f[pos];
            if (pos == 0 && char.IsDigit(c))
            {
                if (pos < f.Length && char.IsDigit(f[pos]))
                {
                    string coeficiente = LerDigitos(f, ref pos);
                    tokens.Add(new Token(TipoToken.Coeficiente, coeficiente, pos - coeficiente.Length));
                }
            }
            else if (char.IsUpper(c))
            {
                int inicio = pos++;
                if (pos < f.Length && char.IsLower(f[pos])) pos++;
                string simbolo = f[inicio..pos];
                tokens.Add(new Token(TipoToken.Simbolo, simbolo, inicio));
                
                if (pos < f.Length && char.IsDigit(f[pos]))
                {
                    string indiceInterno = LerDigitos(f, ref pos);
                    tokens.Add(new Token(TipoToken.IndiceInterno, indiceInterno, pos - indiceInterno.Length));
                }
            }
            else if (c == '(')
            {
                tokens.Add(new Token(TipoToken.AbreParenteses, "(", pos));
                pos++;
            }
            else if (c == ')')
            {
                tokens.Add(new Token(TipoToken.FechaParenteses, ")", pos + 1));
                pos++;

                if (pos >= f.Length || !char.IsDigit(f[pos]))
                    throw new DominioException(CodigoErro.ParentesesDesbalanceados,
                        $"Falta o índice depois do ')' na posição {pos}.");

                string indiceExterno = LerDigitos(f, ref pos);
                tokens.Add(new Token(TipoToken.IndiceExterno, indiceExterno, pos - indiceExterno.Length));
            }
            else if (c == '[')
            {
                tokens.Add(new Token(TipoToken.AbreColchete, "[", pos));
                pos++;
            }
            else if (c == ']')
            {
                tokens.Add(new Token(TipoToken.FechaColchete, "]", pos));
                pos++;
            }
            else if (c == '*')
            {
                tokens.Add(new Token(TipoToken.Hidrato, "*", pos + 1));
                pos++;

                if (pos >= f.Length)
                    throw new DominioException(CodigoErro.ParentesesDesbalanceados,
                        $"Falta o hidrato depois do '*' na posição {pos}.");

                if (char.IsDigit(f[pos]))
                {
                    string coeficienteHidrato = LerDigitos(f, ref pos);
                    tokens.Add(new Token(TipoToken.CoeficienteHidrato, coeficienteHidrato, pos - coeficienteHidrato.Length));
                }
            }
            else
            {
                throw new DominioException(CodigoErro.CaractereInvalido,
                    $"Caractere inválido '{c}' na posição {pos + 1}.");
            }
        }
            return tokens;
    }
    private static string LerDigitos(string f, ref int pos)
    {
        int inicio = pos;
        for (; pos < f.Length && char.IsDigit(f[pos]); pos++) { }
        return f[inicio..pos];
    }
}