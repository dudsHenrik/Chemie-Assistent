namespace ChemieAssistent.Domain;

public readonly record struct Fracao
{
    public long Numerador { get; }
    public long Denominador { get; }

    public Fracao(long numerador, long denominador = 1)
    {
        if (denominador == 0)
            throw new ArgumentException("Denominador não pode ser zero.");
        if (denominador < 0)
        {
            numerador = -numerador;
            denominador = -denominador;
        }
        
        long mdc = CalcularMDC (numerador, denominador);
        Numerador = numerador / mdc;
        Denominador = denominador / mdc;
    }

    public static Fracao operator -(Fracao a) 
        => new(-a.Numerador, a.Denominador);
    public static Fracao operator +(Fracao a, Fracao b) 
        => new Fracao(checked(a.Numerador * b.Denominador + b.Numerador * a.Denominador), checked(a.Denominador * b.Denominador));
    public static Fracao operator -(Fracao a, Fracao b)
            => new Fracao(checked(a.Numerador * b.Denominador - b.Numerador * a.Denominador), checked(a.Denominador * b.Denominador));

    public static Fracao operator *(Fracao a, Fracao b) 
    => new Fracao(checked(a.Numerador * b.Numerador), checked(a.Denominador * b.Denominador));
    public static Fracao operator /(Fracao a, Fracao b) 
    => new Fracao(checked(a.Numerador * b.Denominador), checked(a.Denominador * b.Numerador));

    public bool EhZero => Numerador == 0;
    public bool EhPositivo => Numerador > 0;

    public override string ToString()
    => Denominador == 1 ? $"{Numerador}" : $"{Numerador}/{Denominador}";

    internal static long CalcularMDC(long a, long b)
    {
        while (b != 0)
        {
            long temp = b;
            b = a % b;
            a = temp;
        }
        return Math.Abs(a);
    }
}