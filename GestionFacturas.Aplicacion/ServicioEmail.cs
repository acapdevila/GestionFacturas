﻿using System;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using GestionFacturas.Dominio;

namespace GestionFacturas.Aplicacion
{
    public class ServicioEmail
    {
        public void EnviarMensaje(MensajeEmail mensaje)
        {
            Validar(mensaje);

            var email = GenerarEmail(mensaje);

            foreach (var adjunto in mensaje.Adjuntos)
            {
                email.AdjuntarArchivo(adjunto);
            }           
                    
            Enviar(email);
        }

        public async Task EnviarMensajeAsync(MensajeEmail mensaje)
        {
            Validar(mensaje);

            var email = GenerarEmail(mensaje);

            foreach (var adjunto in mensaje.Adjuntos)
            {
                email.AdjuntarArchivo(adjunto);
            }

            await EnviarAsync(email);
        }

        private MailMessage GenerarEmail(MensajeEmail mensaje)
        {
            var email = new MailMessage
            {
                Subject = mensaje.Asunto,
                From = new MailAddress(mensaje.DireccionRemitente, mensaje.NombreRemitente),
                Body = mensaje.Cuerpo
            };

           email.ReplyToList.Add(mensaje.DireccionRemitente);

            foreach (var destinatario in mensaje.DireccionesDestinatarios)
            {
                email.To.Add(destinatario);
            }

            return email;
        }

        private void Validar(MensajeEmail mensaje)
        {
            if (string.IsNullOrEmpty(mensaje.DireccionRemitente))
                throw new ArgumentException("No se ha indicado el remitente", "DireccionRemitente");

            if (!mensaje.DireccionesDestinatarios.Any())
                throw new ArgumentException("No se ha indicado ningún destinatario", "DireccionesDestinatarios");
        }


        private static void Enviar(MailMessage message)
        {
            var client = new SmtpClient();
            client.Send(message);
        }

        private static async Task EnviarAsync(MailMessage message)
        {
            var client = new SmtpClient();
            await client.SendMailAsync(message);
        }
    }
}
