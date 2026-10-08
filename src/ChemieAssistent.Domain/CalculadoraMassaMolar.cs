namespace ChemieAssistent.Domain;

public sealed class CalculadoraMassaMolar
{
    private readonly IRepositorioElementos _repositorio;

    public CalculadoraMassaMolar(IRepositorioElementos repositorio)
        => _repositorio = repositorio;

    public decimal Calcular(Composto composto)
    {
        decimal massaMolar = 0m;

        foreach (var (simbolo, quantidade) in composto.Atomos)
        {
            var elemento = _repositorio.Obter(simbolo) ?? 
                throw new DominioException(CodigoErro.ElementoInexistente, $"Elemento '{simbolo}' não encontrado.");

            massaMolar += quantidade * elemento.MassaAtomica;
        }

        return Math.Round(massaMolar, 4, MidpointRounding.AwayFromZero);
    }
}