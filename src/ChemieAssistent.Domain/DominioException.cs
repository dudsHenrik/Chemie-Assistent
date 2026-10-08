namespace ChemieAssistent.Domain;

public enum CodigoErro
{
    FormulaVazia,
    CaractereInvalido,
    ParentesesDesbalanceados,
    ElementoInexistente,
    CompostoInvalido,
    SetaAusente,
    SetaDuplicada,
    ReagenteAusente,
    ProdutoAusente,
    CompostoAusente,
    QuantidadeInvalida
}

public sealed class DominioException : Exception
{
    public CodigoErro Codigo { get; }

    public DominioException(CodigoErro codigo, string detalhe) : base(detalhe)
        => Codigo = codigo;
}