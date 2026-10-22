/*  
Balanceia equações químicas por meio de um sistema linear homogêneo,
resolvido pelo método de Gauss-Jordan com aritmética fracionária.
Converte a solução em coeficientes inteiros positivos na menor
proporção equivalente e verifica os coeficientes informados.
*/

namespace  ChemieAssistent.Domain;

public static class Balanceador
{
    public static Equacao Balancear(Equacao eq)
    => BalancearEVerificar(eq).Equacao;

    public static ResultadoBalanceamento BalancearEVerificar(Equacao eq)
    {
        var todos = eq.Reagentes.Concat(eq.Produtos).ToList();
        int qtdReagentes = eq.Reagentes.Count;

        VerificarDivergencia(eq);
        var elementos = ListarElementos(todos);
        var m = MontarMatriz(todos, qtdReagentes, elementos);
        var pivos = Escalonar(m);
        int[] coef = ExtrairCoeficientes(m, pivos, todos.Count);

        var (situacao, divergencias) = VerificarDigitados(todos, coef);

        var reagentes = new List<Composto>();
        var produtos = new List<Composto>();

        for (int j = 0; j < todos.Count; j++) 
        {
            Composto composto = todos[j] with{Coeficiente = coef[j]};

            if (j < qtdReagentes)
                reagentes.Add(composto);
            else
                produtos.Add(composto);
        }

        return new ResultadoBalanceamento(new Equacao(reagentes, produtos, eq.Reversivel), situacao, divergencias);
    }

    private static void VerificarDivergencia(Equacao eq)
    {
        var esquerda = eq.Reagentes.SelectMany(c => c.Atomos.Keys).ToHashSet();
        var direita = eq.Produtos.SelectMany(c => c.Atomos.Keys).ToHashSet();

        if (!esquerda.SetEquals(direita))
            throw new DominioException(CodigoErro.ElementosDivergentes,
                "Há elementos que aparecem em apenas um lado da equação.");
    }

    private static List<string> ListarElementos(List<Composto> todos)
    => todos.SelectMany(c => c.Atomos.Keys).Distinct().ToList();

    private static Fracao[,] MontarMatriz(List<Composto> todos, int qtdReagentes, List<string> elementos)
    {
        var m = new Fracao[elementos.Count, todos.Count];

        for (int i = 0; i < elementos.Count; i++)
            for (int j = 0; j < todos.Count; j++)
            {
                int qtd = todos[j].Atomos.GetValueOrDefault(elementos[i]);
                m[i, j] = new Fracao(j < qtdReagentes ? qtd : -qtd);
            }

        return m;
    }
   
    private static List<int> Escalonar(Fracao[,] m)
    {
        int linhas = m.GetLength(0);
        int colunas = m.GetLength(1);
        var pivos = new List<int>();
        int l = 0;

        for (int c = 0; c < colunas && l < linhas; c++)
        {
            int achada = -1;
            for (int i = l; i < linhas; i++)
            {
                if (!m[i, c].EhZero)
                {
                    achada = i;
                    break;
                }
            }

            if (achada == -1) continue;   // coluna livre

            if (achada != l)
            {
                for (int j = 0; j < colunas; j++)
                {
                    (m[l, j], m[achada, j]) = (m[achada, j], m[l, j]);
                }
            }

            Fracao pivo = m[l, c];

            for (int j = 0; j < colunas; j++)
                m[l, j] = m[l, j] / pivo;

            for (int i = 0; i < linhas; i++)
            {
                if (i == l || m[i, c].EhZero)
                    continue;

                Fracao fator = m[i, c];

                for (int j = 0; j < colunas; j++)
                    m[i, j] = m[i, j] - fator * m[l, j];
            }

            pivos.Add(c);
            l++;
        }

        return pivos;
    }

    private static int[] ExtrairCoeficientes(Fracao[,] m, List<int> pivos, int colunas)
    {
        int qtdLivres = colunas - pivos.Count;

        if (qtdLivres == 0)
            throw new DominioException(CodigoErro.SemSolucao, "Não há colunas livres.");
        if (qtdLivres > 1)
            throw new DominioException(CodigoErro.EquacaoIndeterminada, "Existe mais de uma coluna livre.");

        int livre = -1; 
        for (int c = 0; c < colunas; c++) 
        { 
            if (!pivos.Contains(c)) 
            { 
                livre = c; 
                break; 
            } 
        }
        if (livre == -1) 
            throw new DominioException(CodigoErro.SemSolucao, "Não foi possível identificar a coluna livre.");

        var x = new Fracao[colunas]; 
        
        for (int c = 0; c < colunas; c++) 
            x[c] = new Fracao(0); x[livre] = new Fracao(1); 
        
        for (int k = 0; k < pivos.Count; k++) 
            x[pivos[k]] = -m[k, livre];

        long mmc = 1; 
        
        foreach (Fracao fracao in x) 
        {
            long denominador = fracao.Denominador; 
            mmc = checked(mmc / Fracao.CalcularMDC(mmc, denominador) * denominador); 
        }

        var coeficientes = new int[colunas]; 

        for (int c = 0; c < colunas; c++) 
        { 
            long coeficiente = checked(x[c].Numerador * (mmc / x[c].Denominador)); 
            
            if (coeficiente <= 0) 
                throw new DominioException(CodigoErro.SemSolucao, "A equação não possui uma solução com todos os coeficientes positivos."); 
            
            coeficientes[c] = checked((int)coeficiente); 
        }

        long mdc = coeficientes[0]; 
        
        for (int c = 1; c < colunas; c++) 
            mdc = Fracao.CalcularMDC(mdc, coeficientes[c]); 
        
        for (int c = 0; c < colunas; c++) 
            coeficientes[c] = (int)(coeficientes[c] / mdc); 
        
        return coeficientes;
    }

    private static (SituacaoCoeficientes, List<Divergencia>) VerificarDigitados(List<Composto> todos, int[] esperado)
    {
        int[] digitado = todos.Select(c => c.Coeficiente).ToArray();
        var divergencias = new List<Divergencia>();

        if (digitado.All(d => d == 1))
            return (SituacaoCoeficientes.NaoInformados, divergencias);

        if (digitado.SequenceEqual(esperado))
            return (SituacaoCoeficientes.Corretos, divergencias);

        if (EhProporcional(digitado, esperado))
            return (SituacaoCoeficientes.Proporcionais, divergencias);

        for (int i = 0; i < todos.Count; i++)
            if (digitado[i] != esperado[i])
                divergencias.Add(new Divergencia(
                    i, SemCoeficiente(todos[i].Texto), digitado[i], esperado[i]));

        return (SituacaoCoeficientes.Incorretos, divergencias);
    }

    private static bool EhProporcional(int[] a, int[] b)
    {
        for (int i = 1; i < a.Length; i++)
            if ((long)a[i] * b[0] != (long)a[0] * b[i])
                return false;
        return true;
    }

    private static string SemCoeficiente(string texto)
        => texto.TrimStart("0123456789".ToCharArray());
}