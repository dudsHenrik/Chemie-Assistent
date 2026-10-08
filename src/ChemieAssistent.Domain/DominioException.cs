namespace ChemieAssistent.Domain;

public enum CodigoErro
{
    CaractereInvalido,
    CompostoAusente,
    CompostoInvalido,
    ElementoInexistente,
    ElementosDivergentes,
    EquacaoIndeterminada,
    FormulaVazia,
    ParentesesDesbalanceados,
    ProdutoAusente,
    QuantidadeInvalida,
    ReagenteAusente,
    SemSolucao,
    SetaAusente,
    SetaDuplicada

}

public sealed class DominioException : Exception
{
    public CodigoErro Codigo { get; }

    public DominioException(CodigoErro codigo, string detalhe) : base(detalhe)
        => Codigo = codigo;
}