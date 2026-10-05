namespace ChemieAssistent.Domain;

public interface IRepositorioElementos
{
    Elemento? Obter(string simbolo);
}