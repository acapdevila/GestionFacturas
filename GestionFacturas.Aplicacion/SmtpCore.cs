using CSharpFunctionalExtensions;
using GestionFacturas.Dominio.Infra;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Smtp
{
    internal static class SmtpCore
    {
        internal static MimeMessage ToMimeMessage(this MensajeEmail mensaje, DireccionEmail direcionEnvio)
        {
            var mimeMessage = new MimeMessage();

            foreach (var to in mensaje.Destinatarios)
            {
                mimeMessage.To.Add(new MailboxAddress(to.Nombre, to.Email.Direccion));
            }


            mimeMessage.From.Add(new MailboxAddress(
                direcionEnvio.Nombre,
                direcionEnvio.Email.Direccion));

            mimeMessage.ReplyTo.Add(new MailboxAddress(
                mensaje.Remitente.Nombre,
                mensaje.Remitente.Email.Direccion));


            mimeMessage.Subject = mensaje.Asunto;

            var builder = new BodyBuilder
            {
                TextBody = mensaje.CuerpoTexto,
                HtmlBody = mensaje.CuerpoHtml
            };

            if (mensaje.Adjuntos.Any())
            {
                byte[] fileBytes;
                foreach (var file in mensaje.Adjuntos)
                {
                    if (file.Archivo.Length > 0)
                    {
                        fileBytes = file.Archivo.ToArray();
                        builder.Attachments.Add(file.Nombre, fileBytes, ContentType.Parse(file.MimeType));
                    }
                }
            }
            

            mimeMessage.Body = builder.ToMessageBody();


            return mimeMessage;
        }

        internal static async Task<Result> EnviarEmail(
            MimeMessage mensajeEmail, 
            MailConfig config, 
            CancellationToken cancellation)
        {
            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                try
                {
                    var secureSocketOptions = config.UseSslOrTls ? 
                                SecureSocketOptions.StartTls : 
                                SecureSocketOptions.None;
                    
                    await client.ConnectAsync(config.Host, config.Port, secureSocketOptions, cancellation);
                    
                }
                catch (SmtpCommandException ex)
                {
                     return Result.Failure($"Error al conectar. StatusCode: {ex.StatusCode}. {ex.ToString()}");
                }
                catch (SmtpProtocolException ex)
                {
                    return Result.Failure($"Error de protocolo al intentar conectar. {ex.ToString()}");
                }

                try
                {
                   await client.AuthenticateAsync(config.UserName, config.Password, cancellation);
                }
                catch (AuthenticationException ex)
                {
                     return Result.Failure($"Error de autenticación. Usuario o contraseña no válidos. {ex.ToString()}");
                }
                catch (SmtpCommandException ex)
                {
                    return Result.Failure($"Error Smtp al intentar autenticar. StatusCode: {ex.StatusCode}. {ex.ToString()}");
                }
                catch (SmtpProtocolException ex)
                {
                    return Result.Failure($"Error de protocolo al intentar autenticar. {ex.ToString()}");
                }


                try
                {
                    await client.SendAsync(mensajeEmail, cancellation);
                }
                catch (SmtpCommandException ex)
                {
                    var errorDescripcion = string.Empty;
                    switch (ex.ErrorCode)
                    {
                        case SmtpErrorCode.RecipientNotAccepted:
                            errorDescripcion = $"\tEmail destino no aceptado: {ex.Mailbox}";
                            break;
                        case SmtpErrorCode.SenderNotAccepted:
                            errorDescripcion = $"\tEmail origen not aceptado: {ex.Mailbox}";
                            break;
                        case SmtpErrorCode.MessageNotAccepted:
                            errorDescripcion = "\tMensaje no aceptado.";
                            break;
                    }

                    return Result.Failure(
                        $"Error enviando el mensaje. StatusCode: {ex.StatusCode}. {errorDescripcion}. {ex.ToString()}");
                }
                catch (SmtpProtocolException ex)
                {
                    return Result.Failure($"Error de protocolo al intentar enviar. {ex.ToString()}");
                }
                catch (Exception ex)
                {
                    return Result.Failure($"Error inesperado al intentar enviar. {ex.ToString()}");
                }
                
                await client.DisconnectAsync(true, cancellation);
            }

            return Result.Success();
        }
    }
}
