using CSharpFunctionalExtensions;
using GestionFacturas.Dominio.Seguridad;

namespace GestionFacturas.Dominio
{
    
    public class Usuario : Entity<string>
    {

        protected Usuario()
        {

        }


        public string Email { get; private set; } = string.Empty;

        public string Password { get; private set; } = string.Empty;

        public bool EsPasswordCorrecto(string providedPassword)
        {
            var posiblePassword = Seguridad.Password.Crear(providedPassword);

            if(posiblePassword.IsFailure)
            {
                return false;
            }

            var verificacion = posiblePassword.Value.VerificarPassword(providedPassword);

           return verificacion.IsSuccess;
        }

        public void CambiarPassword(Password password)
        {
            Password = password.PasswordHash;
        }
    }
    
}