namespace GestionFacturas.Dominio;

public enum EstadoFacturaEnum
{
    Borrador = 1,
    Enviada = 2,
    Cobrada = 3,
    Impagada = 4,
    Rectificación = 5,
    EnviadaAHacienda = 6
}