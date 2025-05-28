using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;

namespace GestionFacturas.Dominio.Infra
{
    public class Email : ValueObject
    {
        public static Email Reconstruir(string direccion)
        {
            return new Email(direccion);
        }

        private Email(string direccion)
        {
            Direccion = direccion;
        }

        public static Result<Email> FromString(string email)
        {
            var validacion = ValidarEmail(email);

            if (validacion.IsFailure) 
                return validacion.ConvertFailure<Email>();
            return new Email(email);
        }

        private const string EmailRegularExpression = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";

        public static Result ValidarEmail(string email)
        {
            var reg = new Regex(EmailRegularExpression);

            if (!reg.Match(email).Success)
            {
                return Result.Failure("Email no válido");
            }

            return Result.Success();
        }

        public static implicit operator Email(string direccion)
        {
            return string.IsNullOrEmpty(direccion) ?
                new Email(string.Empty) :
                FromString(direccion).Value;
        }

        public static implicit operator string(Email email)
        {
            return email.Direccion;
        }

        public string Direccion { get; init; }

        public void Deconstruct(out string Direccion)
        {
            Direccion = this.Direccion;
        }

        protected override IEnumerable<IComparable> GetEqualityComponents()
        {
            yield return Direccion;
        }
    }
}
