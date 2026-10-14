namespace ChemieAssistent.Domain;

public enum SituacaoCoeficientes
{
    NaoInformados,   // tudo 1: o usuário não digitou coeficientes
    Corretos,        // idênticos ao balanceado
    Proporcionais,   // balanceada, mas fora da menor proporção (alerta)
    Incorretos       // não balanceia (erro)
}

public sealed record Divergencia(int Indice, string Composto, int Digitado, int Esperado);

public sealed record ResultadoBalanceamento(
    Equacao Equacao,
    SituacaoCoeficientes Situacao,
    IReadOnlyList<Divergencia> Divergencias);