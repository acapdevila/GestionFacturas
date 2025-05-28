using CSharpFunctionalExtensions;

namespace GestionFacturas.Dominio.Infra
{
    public class MensajeEmail
    {
        public static Result<MensajeEmail> Crear(
            string asunto,
            string cuerpoHtml,
            IReadOnlyList<DireccionEmail> destinatarios,
            DireccionEmail remitente,
            IReadOnlyList<ArchivoAdjunto> adjuntos)
        {
            if (string.IsNullOrWhiteSpace(asunto))
                return Result.Failure<MensajeEmail>("El asunto no puede estar vacío");

            if (string.IsNullOrWhiteSpace(cuerpoHtml))
                return Result.Failure<MensajeEmail>("El cuerpo no puede estar vacío");

            if (!destinatarios.Any())
                return Result.Failure<MensajeEmail>("Debe haber al menos un destinatario");

            return new MensajeEmail(
                asunto,
                HtmlToText(cuerpoHtml),
                cuerpoHtml,
                destinatarios.Distinct().ToList(),
                remitente,
                adjuntos
                );
        }


        private MensajeEmail(
            string asunto,
            string cuerpoTexto,
            string cuerpoHtml,
            IReadOnlyList<DireccionEmail> destinatarios, 
            DireccionEmail remitente,
            IReadOnlyList<ArchivoAdjunto> adjuntos)
        {
            Asunto = asunto;
            CuerpoTexto = cuerpoTexto;
            CuerpoHtml = cuerpoHtml;
            Destinatarios = destinatarios;
            Remitente = remitente;
            Adjuntos = adjuntos;
        }

        public string Asunto { get; }
        public string CuerpoTexto { get; }

        public string CuerpoHtml { get; }


        public IReadOnlyList<DireccionEmail> Destinatarios { get; }
        public DireccionEmail Remitente { get;  }

        public IReadOnlyList<ArchivoAdjunto> Adjuntos { get; }

        private static string HtmlToText(string html)
        {
            return html
                .Replace("</br>", Environment.NewLine)
                .Replace("<p>", string.Empty)
                .Replace("</p>", Environment.NewLine)
                .Replace("</li>", Environment.NewLine)
                .Replace("<li>", string.Empty)
                .Replace("<ul>", string.Empty)
                .Replace("</ul>", Environment.NewLine);
        }

    }

    public class DireccionEmail : ValueObject
    {
      
        public static DireccionEmail Crear(string nombre, Email email)
        {
            return new DireccionEmail(nombre, email);
        }

        internal static DireccionEmail FromString(string direccion)
        {
            if (direccion.Contains("<") && direccion.Contains(">"))
            {
                var nombre = direccion.Substring(0, direccion.IndexOf("<", StringComparison.InvariantCultureIgnoreCase)).Trim();
                var mail = direccion.Substring(direccion.IndexOf("<", StringComparison.InvariantCultureIgnoreCase) + 1, direccion.IndexOf(">", StringComparison.InvariantCultureIgnoreCase) - direccion.IndexOf("<", StringComparison.InvariantCultureIgnoreCase) - 1).Trim();
                return new DireccionEmail(nombre, mail);
            }

            return new DireccionEmail(string.Empty, direccion);



        }

        private DireccionEmail(string nombre, Email email)
        {
            Email = email;
            Nombre = nombre;
        }

        public Email Email { get; }
        public string Nombre { get; }


        protected override IEnumerable<IComparable> GetEqualityComponents()
        {
            yield return Email.Direccion;
        }

        
    }
}
