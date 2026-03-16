using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Utils;
using AuthService.Application.Interfaces;

namespace AuthService.Application.Services;

public class EmailService(IConfiguration configuration, ILogger<EmailService> logger) : IEmailService
{
    // 1. Implementación exacta de tu interfaz para Verificación de Email
    public async Task<bool> SendEmailAsync(string email, string username, string token)
    {
        var subject = "Verify your email address";
        var verificationUrl = $"{configuration["AppSettings:FrontendUrl"]}/verify-email?token={token}";

        var htmlBody = $@"
        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
            <h2 style='color: #333;'>Welcome {username}!</h2>
            <p>Please verify your email address by clicking the link below:</p>
            <a href='{verificationUrl}' style='display:inline-block; background-color: #007bff; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>
                Verify Email
            </a>
            <p>If you cannot click the link, copy and paste this URL into your browser:</p>
            <p style='word-break: break-all;'>{verificationUrl}</p>
            <hr/>
            <p style='color: #999; font-size: 12px;'>If you did not create an account, you can safely ignore this email.</p>
        </div>";

        var textBody = $"Welcome {username}!\n\nPlease verify your email by visiting:\n{verificationUrl}\n\nIf you did not create an account, ignore this email.";

        return await ExecuteSendEmailAsync(email, subject, htmlBody, textBody);
    }

    // 2. Implementación exacta de tu interfaz para Reset Password
    public async Task<bool> SendPasswordResetAsync(string email, string username, string token)
    {
        var subject = "Reset your password";
        var resetUrl = $"{configuration["AppSettings:FrontendUrl"]}/reset-password?token={token}";

        var htmlBody = $@"
        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
            <h2 style='color: #333;'>Password Reset Request</h2>
            <p>Hello {username},</p>
            <p>You requested to reset your password. Click the link below to reset it:</p>
            <a href='{resetUrl}' style='display:inline-block; background-color: #dc3545; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>
                Reset Password
            </a>
            <p>This link will expire in <strong>1 hour</strong>.</p>
            <hr/>
            <p style='color: #999; font-size: 12px;'>If you did not request a password reset, you can safely ignore this email.</p>
        </div>";

        var textBody = $"Hello {username},\n\nReset your password by visiting:\n{resetUrl}\n\nThis link expires in 1 hour.\n\nIf you did not request this, ignore this email.";

        return await ExecuteSendEmailAsync(email, subject, htmlBody, textBody);
    }

    // 3. Implementación exacta de tu interfaz para Bienvenida
    public async Task<bool> SendWelcomeEmailAsync(string email, string username)
    {
        var subject = "Welcome to AuthDotnet!";

        var htmlBody = $@"
        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
            <h2 style='color: #28a745;'>Welcome to AuthDotnet, {username}!</h2>
            <p>Your account has been successfully verified and activated.</p>
            <p>You can now log in and start using the platform.</p>
        </div>";

        var textBody = $"Welcome to AuthDotnet, {username}!\n\nYour account has been successfully verified and activated.\nYou can now log in and start using the platform.";

        return await ExecuteSendEmailAsync(email, subject, htmlBody, textBody);
    }

    private async Task<bool> ExecuteSendEmailAsync(string to, string subject, string htmlBody, string textBody)
    {
        var smtpSettings = configuration.GetSection("SmtpSettings");

        try
        {
            var enabled = bool.Parse(smtpSettings["Enabled"] ?? "true");
            if (!enabled)
            {
                logger.LogInformation("Email disabled in configuration. Skipping send");
                return true;
            }

            using var client = new SmtpClient();

            client.Timeout = 10000; // 10 segundos máximo
            client.CheckCertificateRevocation = false;
            client.ServerCertificateValidationCallback = (s, c, h, e) => true;

            var host = smtpSettings["Host"];
            var port = int.Parse(smtpSettings["Port"] ?? "587");
            var user = smtpSettings["Username"];
            var pass = smtpSettings["Password"];

            if (port == 465)
            {
                await client.ConnectAsync(host, port, SecureSocketOptions.SslOnConnect);
            }
            else
            {
                await client.ConnectAsync(host, port, SecureSocketOptions.StartTls);
            }

            await client.AuthenticateAsync(user, pass);

            // Construcción del mensaje con headers anti-spam
            var message = new MimeMessage();

            var fromEmail = smtpSettings["FromEmail"] ?? user;
            var fromName  = smtpSettings["FromName"]  ?? "Auth System";

            message.From.Add(new MailboxAddress(fromName, fromEmail));
            message.To.Add(new MailboxAddress("", to));
            message.Subject = subject;

            // MessageId único — reduce probabilidad de spam
            message.MessageId = MimeUtils.GenerateMessageId();

            // Header X-Mailer para identificar el cliente
            message.Headers.Add("X-Mailer", "AuthDotnet/1.0");

            // Construir cuerpo con versión HTML + texto plano
            // Tener ambas versiones es clave para no caer en spam
            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = htmlBody,
                TextBody = textBody
            };
            message.Body = bodyBuilder.ToMessageBody();

            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            logger.LogInformation("Email sent successfully to {Email}", to);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error crítico enviando correo a {Email}: {Message}", to, ex.Message);
            return false;
        }
    }
}