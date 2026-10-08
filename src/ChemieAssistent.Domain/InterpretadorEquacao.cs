namespace ChemieAssistent.Domain;

public static class InterpretadorEquacao
{
    public static Equacao Interpretar(string texto)
    {
        EquacaoTexto eq = LeitorEquacao.Ler(texto);

        var reagentes = LerCompostos(eq.Reagentes);
        var produtos = LerCompostos(eq.Produtos);

        return new Equacao(reagentes, produtos, eq.Reversivel);
    }

    private static List<Composto> LerCompostos(List<string> textos)
    {
        var compostos = new List<Composto>();

        for (int i = 0; i < textos.Count; i++)
            compostos.Add(ParserFormula.Ler(textos[i]));

        return compostos;
    }
}