using ChemieAssistent.Domain;

namespace ChemieAssistent.Domain.Tests;

internal sealed class IRepositorioElementosFake : IRepositorioElementos
{
    private readonly Dictionary<string, Elemento> _elementos;

    public IRepositorioElementosFake(params Elemento[] elementos)
        => _elementos = elementos.ToDictionary(e => e.Simbolo);

    // Mesmos valores da tabela Atomo
    public static IRepositorioElementosFake Padrao() => new(
        new Elemento("H", "Hidrogênio", 1.0080m),
        new Elemento("C", "Carbono", 12.0110m),
        new Elemento("O", "Oxigênio", 15.9990m),
        new Elemento("S", "Enxofre", 32.0600m),
        new Elemento("Ca", "Cálcio", 40.0780m),
        new Elemento("Fe", "Ferro", 55.8450m),
        new Elemento("Cu", "Cobre", 63.5460m));

    public Elemento? Obter(string simbolo)
        => _elementos.TryGetValue(simbolo, out var e) ? e : null;
}