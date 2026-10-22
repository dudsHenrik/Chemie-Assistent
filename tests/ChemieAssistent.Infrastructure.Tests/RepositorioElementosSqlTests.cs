namespace ChemieAssistent.Infrastructure.Tests;

/*
Valida a integração do repositório com o SQL Server,
verificando a recuperação de elementos cadastrados
e o retorno nulo para símbolos inexistentes.
*/
public class RepositorioElementosSqlTests
{
    private const string ConnectionString =
        "Server=(localdb)\\MSSQLLocalDB;Database=ChemieAssistent;Trusted_Connection=True;TrustServerCertificate=True;";

    private readonly RepositorioElementosSql _repositorio = new(ConnectionString);

    [Fact]
    [Trait("Categoria", "Integracao")]
    public void Obter_ferro_devolve_nome_e_massa_atomica()
    {
        var elemento = _repositorio.Obter("Fe");

        Assert.NotNull(elemento);
        Assert.Equal("Ferro", elemento.Nome);
        Assert.Equal(55.845m, elemento.MassaAtomica);
    }

    [Fact]
    [Trait("Categoria", "Integracao")]
    public void Obter_simbolo_inexistente_devolve_null()
    {
        var elemento = _repositorio.Obter("Xx");

        Assert.Null(elemento);
    }
}