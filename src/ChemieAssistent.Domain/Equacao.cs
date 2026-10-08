namespace ChemieAssistent.Domain;

public sealed record Composto(string Texto, int Coeficiente, int CoeficienteHidrato, IReadOnlyDictionary<string, int> Atomos);

public sealed record Equacao(List<Composto> Reagentes, List<Composto> Produtos, bool Reversivel);