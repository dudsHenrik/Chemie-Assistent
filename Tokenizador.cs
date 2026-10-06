namespace ChemieAssistent.Domain;

public static class Tokenizador
{
    public static List<Token> Ler(string f)
    {
        var tokens = new List<Token>();
        for (int pos = 0; pos < f.Length; pos++)
        {
            char c = f[pos];
            if (pos = 0 && char.IsDigit(c))
            {
                int inicio = pos++;
                if (pos < f.Length && char.IsDigit(f[pos])) pos++;
                int coeficiente = int.Parse(f[inicio..pos]);
                tokens .Add(new Token(TipoToken.Coeficiente, coeficiente.ToString(), inicio));
            }
            else if (char.IsUpper(c))
            {
                int inicio = pos++;
                if (pos < f.Length && char.IsLower(f[pos])) pos++;   // símbolo: 1 maiúscula + 0 ou 1 minúscula
                string simbolo = f[inicio..pos];
                tokens.Add(new Token(TipoToken.Simbolo, simbolo, inicio));
            }
            else if (char.IsDigit(c))
            {
                int inicio = pos++;
                if (pos < f.Length && char.IsDigit(f[pos])) pos++;
                int indiceInterno = int.Parse(f[inicio..pos]);
                tokens.Add(new Token(TipoToken.IndiceInterno, indiceInterno.ToString(), inicio));
            }
            else if (c == '(')
            {
                string abreParenteses = f[pos];
                tokens.Add(new Token(TipoToken.AbreParenteses, abreParenteses, pos));
                pos++;
            }
            else if (c == ')')
            {   
                string fechaParenteses = f[pos];
                tokens.Add(new Token(TipoToken.FechaParenteses, fechaParenteses, pos));

                int inicio = pos++;
                if (pos < f.Length && char.IsDigit(f[inicio]))
                {
                    pos++;
                    int indiceExterno = int.Parse(f[inicio..pos]);
                    tokens.Add(new Token(TipoToken.IndiceExterno, indiceExterno.ToString(), inicio));
                }
                else
                {
                    throw new DominioException(CodigoErro.ParentesesDesbalanceados,
                        "Aninhamento sem indice externo na posição {0}", pos);
                }

            }
            else if (c == '[')
            {
                string abreColchete = f[pos];
                tokens.Add(new Token(TipoToken.AbreColchete, abreColchete, pos));
                pos++;
            }
            else if (c == ']')
            {
                string fechaColchete = f[pos];
                tokens.Add(new Token(TipoToken.FechaColchete, fechaColchete, pos));
                pos++;
            }
            else
            {
                throw new DominioException(CodigoErro.CaractereInvalido,
                    $"Caractere inválido '{c}' na posição {pos + 1}.");
            }
        }
            return tokens;
    }
}