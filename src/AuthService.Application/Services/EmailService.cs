using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using AuthService.Domain.Interface; // Cambié a Domain.Interface que es donde definiste la interfaz

namespace AuthService.Application.Services;

public class EmailService(IConfiguration configuration, ILogger<EmailService> logger) : IEmailService
{
    // MÉTODO 1: Para Verificación (Cumple la interfaz)
    public async Task<bool> SendEmailAsync(string email, string username, string token)
    {
        var subject = "Verify your email address";
        var verificationUrl = $"{configuration["AppSettings:FrontendUrl"]}/verify-email?token={token}";

        var body = $@"
            <h2>Welcome {username}!</h2>
            <p>Please verify your email address by clicking the link below:</p>
            <a href='{verificationUrl}' style='background-color: #007bff; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>
                Verify Email
            </a>
            <p>{verificationUrl}</p>
            <p>This link will expire in 24 hours.</p>
            <p>If you didn't create an account, please ignore this email.</p>";

        // LLAMADA AL MÉTODO DE LÓGICA (ExecuteSend)
        return await ExecuteSendEmailAsync(email, subject, body);
    }

    // MÉTODO 2: Para Reset (Cumple la interfaz)
    public async Task<bool> SendPasswordResetAsync(string email, string username, string token)
    {
        var subject = "Reset your password";
        var resetUrl = $"{configuration["AppSettings:FrontendUrl"]}/reset-password?token={token}";

        var body = $@"
            <h2>Password Reset Request</h2>
            <p>Hello {username},</p>
            <p>You requested to reset your password. Click the link below to reset it:</p>
            <a href='{resetUrl}' style='background-color: #dc3545; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>
                Reset Password
            </a>
            <p>{resetUrl}</p>
            <p>This link will expire in 1 hour.</p>";

        return await ExecuteSendEmailAsync(email, subject, body);
    }

    // MÉTODO 3: Para Bienvenida (Cumple la interfaz)
    public async Task<bool> SendWelcomeEmailAsync(string email, string username)
    {
        var subject = "Welcome to AuthDotnet!";

        var body = $@"
            <h2>Welcome to AuthDotnet, {username}!</h2>
            <p>Your account has been successfully verified and activated.</p>
            <p>Thank you for joining us!</p>";

        return await ExecuteSendEmailAsync(email, subject, body);
    }
    
    // MÉTODO 4: TU LÓGICA ORIGINAL (Renombrado para que no choque y no sea recursivo)
    private async Task<bool> ExecuteSendEmailAsync(string to, string subject, string body)
    {
        var smtpSettings = configuration.GetSection("SmtpSettings");

        try
        {
            var enabled = bool.Parse(smtpSettings["Enabled"] ?? "true");
            if (!enabled)
            {
                logger.LogInformation("Email disabled in configuration. Skipping send");
                return false;
            }

            var host = smtpSettings["Host"];
            var portString = smtpSettings["Port"];
            var username = smtpSettings["Username"];
            var password = smtpSettings["Password"];
            var fromEmail = smtpSettings["FromEmail"];
            var fromName = smtpSettings["FromName"];

            if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                logger.LogError("SMTP settings are not properly configured");
                throw new InvalidOperationException("SMTP settings are not properly configured");
            }

            var port = int.Parse(portString ?? "587");
            using var client = new SmtpClient();
            client.Timeout = int.Parse(smtpSettings["Timeout"] ?? "30000");
            client.CheckCertificateRevocation = false;
            client.ServerCertificateValidationCallback = (s, c, h, e) => true;

            try
            {
                var useImplicitSsl = bool.Parse(smtpSettings["UseImplicitSsl"] ?? "false");

                if (useImplicitSsl || port == 465)
                    await client.ConnectAsync(host, port, SecureSocketOptions.SslOnConnect);
                else if (port == 587)
                    await client.ConnectAsync(host, port, SecureSocketOptions.StartTls);
                else
                    await client.ConnectAsync(host, port, SecureSocketOptions.Auto);

                await client.AuthenticateAsync(username, password);

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(fromName ?? "Auth System", fromEmail ?? "noreply@auth.com"));
                message.To.Add(new MailboxAddress("", to));
                message.Subject = subject;
                message.Body = new TextPart("html") { Text = body };

                await client.SendAsync(message);
                logger.LogInformation("Email sent successfully");
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to send email");
                throw;
            }
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send email");
            if (bool.Parse(smtpSettings["UseFallback"] ?? "false")) return true;
            throw;
        }
    }
}