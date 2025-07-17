namespace GestionFacturas.Dominio.Seguridad;

public record PasswordVerificado
{   
    internal static PasswordVerificado Crear(
                bool requiereActualizar)
    {
        return new PasswordVerificado(
                    requiereActualizar);
    }

    private PasswordVerificado(bool requiereActualizar)
    {
        this.RequiereActualizar = requiereActualizar;
        
    }

    public bool RequiereActualizar { get; }

    


}