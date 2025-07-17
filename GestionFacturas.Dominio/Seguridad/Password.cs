using CSharpFunctionalExtensions;

namespace GestionFacturas.Dominio.Seguridad
{
    public record Password
    {   
        public static Result<Password> Crear(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return Result.Failure<Password>("Indica la contraseña");
            
            return new Password(PasswordHasher.HashPassword(password));
        }

        public static Password Reconstruir(string passwordHash)
        {
            return new Password(passwordHash);
        }

        private Password(string PasswordHash)
        {
            this.PasswordHash = PasswordHash;
        }

        public string PasswordHash { get; }

        internal Result<Password> CambiarPassword(string password)
        {
            return Crear(password);
        }

        public Result<PasswordVerificado> VerificarPassword(string providedPassword)
        {
            var verificacion = VerificarEsPasswordCorrecto(providedPassword);

            if (verificacion == PasswordVerificationResult.Failed)
            {
                return Result.Failure<PasswordVerificado>("Contraseña incorrecta.");
            }

            return PasswordVerificado.Crear(
                requiereActualizar:
                verificacion is PasswordVerificationResult.SuccessRehashNeeded
                );
        }

        private PasswordVerificationResult VerificarEsPasswordCorrecto(string providedPassword)
        {
            return PasswordHasher.VerifyHashedPassword(PasswordHash, providedPassword);
        }
    }
}
