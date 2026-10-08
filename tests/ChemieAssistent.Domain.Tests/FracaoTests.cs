using ChemieAssistent.Domain;

public class FracaoTests
{
    [Theory]
    [InlineData(2, 4, 1, 2)]
    [InlineData(6, 9, 2, 3)]
    [InlineData(10, 5, 2, 1)]
    [InlineData(0, 5, 0, 1)]
    public void Deve_SimplificarFracao(
        long numerador,
        long denominador,
        long numeradorEsperado,
        long denominadorEsperado)
    {
        var fracao = new Fracao(numerador, denominador);

        Assert.Equal(numeradorEsperado, fracao.Numerador);
        Assert.Equal(denominadorEsperado, fracao.Denominador);
    }

    [Theory]
    [InlineData(1, -2, -1, 2)]
    [InlineData(-1, -2, 1, 2)]
    [InlineData(-2, 4, -1, 2)]
    public void Deve_NormalizarSinal(
        long numerador,
        long denominador,
        long numeradorEsperado,
        long denominadorEsperado)
    {
        var fracao = new Fracao(numerador, denominador);

        Assert.Equal(numeradorEsperado, fracao.Numerador);
        Assert.Equal(denominadorEsperado, fracao.Denominador);
    }

    [Fact]
    public void Deve_LancarExcecao_QuandoDenominadorForZero()
    {
        var excecao = Assert.Throws<ArgumentException>(
            () => new Fracao(1, 0));

        Assert.Equal("Denominador não pode ser zero.", excecao.Message);
    }

    [Theory]
    [InlineData(1, 2, -1, 2)]
    [InlineData(-1, 2, 1, 2)]
    [InlineData(3, 4, -3, 4)]
    [InlineData(0, 5, 0, 1)]
    public void Deve_Negar(
    long numerador,
    long denominador,
    long numeradorEsperado,
    long denominadorEsperado)
    {
        var fracao = new Fracao(numerador, denominador);

        var resultado = -fracao;

        Assert.Equal(
            new Fracao(numeradorEsperado, denominadorEsperado),
            resultado);
    }

    [Theory]
    [InlineData(1, 2, 1, 3, 5, 6)]
    [InlineData(1, 2, 1, 2, 1, 1)]
    [InlineData(2, 3, 1, 6, 5, 6)]
    [InlineData(-1, 2, 1, 2, 0, 1)]
    public void Deve_Somar(
        long aNumerador,
        long aDenominador,
        long bNumerador,
        long bDenominador,
        long numeradorEsperado,
        long denominadorEsperado)
    {
        var a = new Fracao(aNumerador, aDenominador);
        var b = new Fracao(bNumerador, bDenominador);

        var resultado = a + b;

        Assert.Equal(
            new Fracao(numeradorEsperado, denominadorEsperado),
            resultado);
    }

    [Theory]
    [InlineData(1, 2, 1, 3, 1, 6)]
    [InlineData(1, 2, 1, 2, 0, 1)]
    [InlineData(2, 3, 1, 6, 1, 2)]
    public void Deve_Subtrair(
        long aNumerador,
        long aDenominador,
        long bNumerador,
        long bDenominador,
        long numeradorEsperado,
        long denominadorEsperado)
    {
        var a = new Fracao(aNumerador, aDenominador);
        var b = new Fracao(bNumerador, bDenominador);

        var resultado = a - b;

        Assert.Equal(
            new Fracao(numeradorEsperado, denominadorEsperado),
            resultado);
    }

    [Theory]
    [InlineData(1, 2, 2, 3, 1, 3)]
    [InlineData(2, 3, 3, 4, 1, 2)]
    [InlineData(-1, 2, 2, 3, -1, 3)]
    [InlineData(0, 5, 2, 3, 0, 1)]
    public void Deve_Multiplicar(
        long aNumerador,
        long aDenominador,
        long bNumerador,
        long bDenominador,
        long numeradorEsperado,
        long denominadorEsperado)
    {
        var a = new Fracao(aNumerador, aDenominador);
        var b = new Fracao(bNumerador, bDenominador);

        var resultado = a * b;

        Assert.Equal(
            new Fracao(numeradorEsperado, denominadorEsperado),
            resultado);
    }

    [Theory]
    [InlineData(1, 2, 2, 3, 3, 4)]
    [InlineData(2, 3, 1, 6, 4, 1)]
    [InlineData(-1, 2, 2, 3, -3, 4)]
    public void Deve_Dividir(
        long aNumerador,
        long aDenominador,
        long bNumerador,
        long bDenominador,
        long numeradorEsperado,
        long denominadorEsperado)
    {
        var a = new Fracao(aNumerador, aDenominador);
        var b = new Fracao(bNumerador, bDenominador);

        var resultado = a / b;

        Assert.Equal(
            new Fracao(numeradorEsperado, denominadorEsperado),
            resultado);
    }


    [Theory]
    [InlineData(0, 5, true)]
    [InlineData(0, 1, true)]
    [InlineData(1, 5, false)]
    [InlineData(-1, 5, false)]
    public void EhZero_Deve_IndicarCorretamente(
        long numerador,
        long denominador,
        bool esperado)
    {
        var fracao = new Fracao(numerador, denominador);

        Assert.Equal(esperado, fracao.EhZero);
    }

    [Theory]
    [InlineData(1, 5, true)]
    [InlineData(5, 1, true)]
    [InlineData(-1, 5, false)]
    [InlineData(0, 5, false)]
    public void EhPositivo_Deve_IndicarCorretamente(
        long numerador,
        long denominador,
        bool esperado)
    {
        var fracao = new Fracao(numerador, denominador);

        Assert.Equal(esperado, fracao.EhPositivo);
    }
}