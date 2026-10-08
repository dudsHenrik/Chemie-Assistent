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

    public enum UnidadeMassa { Grama, Quilograma }

    public sealed record EntradaMassa(int IndiceComposto, decimal Valor, UnidadeMassa Unidade);

    public sealed record LinhaResultado(
        Composto Composto, bool EhReagente, decimal MassaMolar, decimal Mols, decimal MassaGramas);

    private static decimal ParaGramas(decimal valor, UnidadeMassa unidade)
    => unidade switch
    {
        UnidadeMassa.Grama => valor,
        UnidadeMassa.Quilograma => valor * 1000m,
        _ => throw new ArgumentOutOfRangeException(nameof(unidade))
    };
}