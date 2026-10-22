/*
Implementa o acesso aos dados dos elementos químicos no SQL Server,
consultando o elemento pelo símbolo e convertendo o resultado em um
objeto de domínio.
*/

using ChemieAssistent.Domain;
using Microsoft.Data.SqlClient;

namespace ChemieAssistent.Infrastructure;

public sealed class RepositorioElementosSql : IRepositorioElementos
{
    private readonly string _connectionString;

    public RepositorioElementosSql(string connectionString)
        => _connectionString = connectionString;

    public Elemento? Obter(string simbolo)
    {
        const string sql =
            "SELECT Simbolo, Nome, MassaAtomica FROM Atomo WHERE Simbolo = @simbolo";

        using var conexao = new SqlConnection(_connectionString);
        using var comando = new SqlCommand(sql, conexao);

        comando.Parameters.AddWithValue("@simbolo", simbolo);
        conexao.Open();
        using var leitor = comando.ExecuteReader();
        if (leitor.Read())
        {
             return new Elemento(
                 leitor.GetString(0),     // Simbolo
                 leitor.GetString(1),     // Nome
                 leitor.GetDecimal(2));   // MassaAtomica
        }

        return null;
    }
}