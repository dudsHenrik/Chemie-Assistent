using ChemieAssistent.Domain;

public sealed class Estequiometria
{
    private readonly CalculadoraMassaMolar _massaMolar;

    public Estequiometria(CalculadoraMassaMolar massaMolar)
        => _massaMolar = massaMolar;

    public List<LinhaResultado> Calcular(Equacao eq, EntradaMassa entrada)
    {
        var todos = eq.Reagentes.Concat(eq.Produtos).ToList();
        int qtdReagentes = eq.Reagentes.Count;

        if (entrada.IndiceComposto < 0 || entrada.IndiceComposto >= todos.Count)
            throw new DominioException(CodigoErro.CompostoAusente, "Índice do composto inválido.");

        if (entrada.Valor <= 0)
            throw new DominioException(CodigoErro.QuantidadeInvalida, "A massa informada deve ser maior que zero.");

        decimal massaInformada = ParaGramas( entrada.Valor, entrada.Unidade);
        var massasMolares = todos .Select(c => _massaMolar.Calcular(c)) .ToList(); 
        int indiceEntrada = entrada.IndiceComposto; 
        decimal mmEntrada = massasMolares[indiceEntrada]; 
        
        if (mmEntrada <= 0) 
            throw new DominioException( CodigoErro.SemSolucao, "A massa molar do composto deve ser positiva."); 
        
        decimal molsEntrada = massaInformada / mmEntrada; 
        int coefEntrada = todos[indiceEntrada].Coeficiente; 
        
        if (coefEntrada <= 0) 
            throw new DominioException( CodigoErro.SemSolucao, "O coeficiente estequiométrico deve ser positivo."); 
        
        
        var resultados = new List<LinhaResultado>(); 
        
        for (int i = 0; i < todos.Count; i++) 
        {
            Composto composto = todos[i]; 
            decimal mm = massasMolares[i]; 
            
            if (composto.Coeficiente <= 0) 
                throw new DominioException( CodigoErro.SemSolucao, "O coeficiente estequiométrico deve ser positivo."); 

            decimal mols = molsEntrada * composto.Coeficiente / coefEntrada; 
            decimal massaGramas = mols * mm; 
            resultados.Add(new LinhaResultado( composto, i < qtdReagentes, mm, mols, massaGramas)); 
        } 
        return resultados;
    }

    public enum UnidadeMassa 
    { 
        Grama, 
        Quilograma 
    }

    public sealed record EntradaMassa(
        int IndiceComposto, 
        decimal Valor, 
        UnidadeMassa Unidade);

    public sealed record LinhaResultado(
        Composto Composto, 
        bool EhReagente, 
        decimal MassaMolar, 
        decimal Mols, 
        decimal MassaGramas);

    public sealed record LinhaLimitante(
        Composto Composto,
        bool EhReagente,
        decimal MassaMolar,
        decimal Mols,
        decimal MassaGramas,
        decimal ExcedenteMols,
        decimal ExcedenteGramas);

    public sealed record ResultadoLimitante(
        int IndiceLimitante,
        List<LinhaLimitante> Linhas);

    public ResultadoLimitante CalcularLimitante(Equacao eq, IReadOnlyList<EntradaMassa> entradas)
    {
        var todos = eq.Reagentes.Concat(eq.Produtos).ToList();
        int qtdReagentes = eq.Reagentes.Count;

        if (entradas.Count < 2)
            throw new DominioException(CodigoErro.EntradaInvalida,
                "Informe a massa de pelo mensos dois reagentes para identificar o limitante.");

        var massasMolares = todos.Select(c => _massaMolar.Calcular(c)).ToList();
        var molsInformados = new Dictionary<int, decimal>();

        foreach (var e in entradas)
        {
            if (e.IndiceComposto < 0 || e.IndiceComposto >= todos.Count)
                throw new DominioException(CodigoErro.CompostoAusente, "Índice do composto inválido.");

            if (e.IndiceComposto >= qtdReagentes)
                throw new DominioException(CodigoErro.EntradaInvalida, 
                    "O reagente limitante só pode ser calculado entre reagentes; há uma massa de produto.");

            if (molsInformados.ContainsKey(e.IndiceComposto))
                throw new DominioException(CodigoErro.EntradaInvalida,
                    "O mesmo reagente foi informado mais de uma vez.");

            if (e.Valor <= 0)
                throw new DominioException(CodigoErro.QuantidadeInvalida,
                    "A massa informada deve ser maior que zero.");

            if (todos[e.IndiceComposto].Coeficiente <= 0 || massasMolares[e.IndiceComposto] <= 0)
                throw new DominioException(CodigoErro.SemSolucao,
                    "Coeficiente e massa molar devem ser positivos.");

            molsInformados[e.IndiceComposto] =
                ParaGramas(e.Valor, e.Unidade) / massasMolares[e.IndiceComposto];
        }

        int limitante = molsInformados
            .MinBy(p => p.Value / todos[p.Key].Coeficiente).Key;
        decimal extensao = molsInformados[limitante] / todos[limitante].Coeficiente;

        var linhas = new List<LinhaLimitante>();
        for (int i = 0; i < todos.Count; i++)
        {
            decimal mols = extensao * todos[i].Coeficiente;
            decimal excedenteMols = molsInformados.TryGetValue(i, out decimal informado)
                ? informado - mols
                : 0m;

            linhas.Add(new LinhaLimitante(
                todos[i], i < qtdReagentes, massasMolares[i],
                mols, mols * massasMolares[i],
                excedenteMols, excedenteMols * massasMolares[i]));
        }
        return new ResultadoLimitante(limitante, linhas);
    }

    private static decimal ParaGramas(decimal valor, UnidadeMassa unidade)
    => unidade switch
    {
        UnidadeMassa.Grama => valor,
        UnidadeMassa.Quilograma => valor * 1000m,
        _ => throw new ArgumentOutOfRangeException(nameof(unidade))
    };

}