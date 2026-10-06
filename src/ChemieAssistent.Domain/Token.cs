namespace ChemieAssistent.Domain;

public enum TipoToken
{
    Coeficiente,
    Simbolo,
    IndiceInterno,
    IndiceExterno,
    AbreParenteses,
    FechaParenteses,
    AbreColchete,
    FechaColchete,
    Hidrato,
    CoeficienteHidrato
}

public sealed record Token(TipoToken Tipo, string Texto, int Posicao);