using GestionFacturas.Dominio;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.IO;
using System;
using GestionFacturas.Dominio.Infra;
using Smtp;

namespace GestionFacturas.Aplicacion
{
    public interface IServicioEmail
    {
        void EnviarMensaje(MensajeEmail mensaje);
        Task EnviarMensajeAsync(MensajeEmail mensaje);
    }
    public class MailSettings
    {
        public string DeliveryMethod { get; set; } = string.Empty;
        public string PickupDirectoryLocation { get; set; } = string.Empty;
        public string From { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }

        public string ReplyTo { get; set; } = string.Empty;

        public string[] ReplyToList() =>
            ReplyTo.Split(';', StringSplitOptions.RemoveEmptyEntries |
                               StringSplitOptions.TrimEntries);

        public string DisplayNames { get; set; } = string.Empty;

        public string[] DisplayNamesList() =>
            DisplayNames.Split(';', StringSplitOptions.RemoveEmptyEntries |
                               StringSplitOptions.TrimEntries);

    }

    public class ServicioEmailMailKid : IServicioEmail
    {
        private readonly MailSettings _mailSettings;

        public ServicioEmailMailKid(MailSettings mailSettings)
        {
            _mailSettings = mailSettings;
        }

        public void EnviarMensaje(MensajeEmail mensaje)
        {
            var email = GenerarEmail(mensaje);
                    
            Enviar(email);
        }

        public async Task EnviarMensajeAsync(MensajeEmail mensaje)
        {
            var email = GenerarEmail(mensaje);
            
            await EnviarAsync(email);
        }

        private MimeMessage GenerarEmail(MensajeEmail mensaje)
        {
            var direccionEnvio = DireccionEmail.Crear(
                mensaje.Remitente.Nombre,
                mensaje.Remitente.Email);

            var email = mensaje.ToMimeMessage(direccionEnvio);
            return email;
        }
        



        private void Enviar(MimeMessage email)
        {
            if (_mailSettings.DeliveryMethod == "SpecifiedPickupDirectory")
            {
                SaveToPickupDirectory(email, _mailSettings.PickupDirectoryLocation);
                return;
            }
            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.UserName, _mailSettings.Password);
            smtp.Send(email);
            smtp.Disconnect(true);
        }

        private async Task EnviarAsync(MimeMessage email)
        {
            if (_mailSettings.DeliveryMethod == "SpecifiedPickupDirectory")
            {
                SaveToPickupDirectory(email, _mailSettings.PickupDirectoryLocation);
                return;
            }

            // https://codewithmukesh.com/blog/send-emails-with-aspnet-core/

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_mailSettings.UserName, _mailSettings.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }

        public static void SaveToPickupDirectory(MimeMessage message, string pickupDirectory)
        {
            do
            {
                // Generate a random file name to save the message to.
                var path = Path.Combine(pickupDirectory, Guid.NewGuid().ToString() + ".eml");
                Stream stream;

                try
                {
                    // Attempt to create the new file.
                    stream = File.Open(path, FileMode.CreateNew);
                }
                catch (IOException)
                {
                    // If the file already exists, try again with a new Guid.
                    if (File.Exists(path))
                        continue;

                    // Otherwise, fail immediately since it probably means that there is
                    // no graceful way to recover from this error.
                    throw;
                }

                try
                {
                    using (stream)
                    {
                        // IIS pickup directories expect the message to be "byte-stuffed"
                        // which means that lines beginning with "." need to be escaped
                        // by adding an extra "." to the beginning of the line.
                        //
                        // Use an SmtpDataFilter "byte-stuff" the message as it is written
                        // to the file stream. This is the same process that an SmtpClient
                        // would use when sending the message in a `DATA` command.
                        using (var filtered = new FilteredStream(stream))
                        {
                            filtered.Add(new SmtpDataFilter());

                            // Make sure to write the message in DOS (<CR><LF>) format.
                            var options = FormatOptions.Default.Clone();
                            options.NewLineFormat = NewLineFormat.Dos;

                            message.WriteTo(options, filtered);
                            filtered.Flush();
                            return;
                        }
                    }
                }
                catch
                {
                    // An exception here probably means that the disk is full.
                    //
                    // Delete the file that was created above so that incomplete files are not
                    // left behind for IIS to send accidentally.
                    File.Delete(path);
                    throw;
                }
            } while (true);
        }
    }
}
