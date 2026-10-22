namespace ChemieAssistent.Domain;

// Representa a situação dos coeficientes informados em relação
// aos coeficientes calculados pelo balanceamento.
public enum SituacaoCoeficientes
{
    NaoInformados,  // Todos iguais a 1: coeficientes não informados explicitamente.
    Corretos,       // Idênticos aos coeficientes balanceados.
    Proporcionais,  // Proporção correta, mas não reduzida à menor razão inteira.
    Incorretos      // Não correspondem à proporção estequiométrica esperada.
}

// Registra a diferença entre um coeficiente informado e o esperado.
public sealed record Divergencia(
    int Indice,
    string Composto,
    int Digitado,
    int Esperado);

// Agrupa a equação balanceada e a avaliação dos coeficientes originalmente informados.
public sealed record ResultadoBalanceamento(
    Equacao Equacao,
    SituacaoCoeficientes Situacao,
    IReadOnlyList<Divergencia> Divergencias);