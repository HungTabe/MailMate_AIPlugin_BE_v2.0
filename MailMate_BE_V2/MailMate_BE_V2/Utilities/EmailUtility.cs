using System;
using System.IO;
using System.Threading.Tasks;
using MailMate_BE_V2.Controllers;
using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using RazorLight;
using MailMate_BE_V2.DTOs;

namespace MailMate_BE_V2.Utilities
{
    public class EmailUtility
    {
        private readonly IConfiguration _configuration;
        private readonly RazorLightEngine _razorEngine;

        public EmailUtility(IConfiguration configuration)
        {
            _configuration = configuration;
            _razorEngine = new RazorLightEngineBuilder()
                .UseFileSystemProject(Path.Combine(Directory.GetCurrentDirectory(), "Templates"))
                .UseMemoryCachingProvider()
                .Build();
        }

        public async Task SendConsultationEmailAsync(ConsultationRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "ConsultationRequest cannot be null");
            }

            var emailConfig = _configuration.GetSection("EmailSettings");
            var senderEmail = emailConfig["SenderEmail"];
            var senderName = emailConfig["SenderName"];
            var smtpHost = emailConfig["SmtpHost"];
            var smtpPort = int.Parse(emailConfig["SmtpPort"]);
            var smtpUsername = emailConfig["SmtpUsername"];
            var smtpPassword = emailConfig["SmtpPassword"];

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(senderName, senderEmail));
            message.To.Add(new MailboxAddress(request.FullName ?? "Customer", request.Email));
            message.Subject = $"MAILMate Consultation for {request.PlanType ?? "Unknown"} Plan";

            // Default fallback templates
            var defaultHtmlTemplate = @"<!DOCTYPE html>
            <html lang=""en"">
            <head>
                <meta charset=""UTF-8"">
                <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                <style>
                    body { font-family: Arial, sans-serif; color: #333; margin: 0; padding: 0; }
                    .container { max-width: 600px; margin: 0 auto; padding: 20px; background-color: #f9f9f9; }
                    .content { background-color: white; padding: 20px; border-radius: 5px; }
                </style>
            </head>
            <body>
                <div class=""container"">
                    <div class=""content"">
                        <h2>Dear Customer,</h2>
                        <p>Thank you for your interest in the MAILMate plan. We will contact you soon with more details.</p>
                        <p>Best regards,<br>The MAILMate Team</p>
                    </div>
                </div>
            </body>
            </html>";

            string htmlBody = defaultHtmlTemplate;
            string textBody = $@"Dear {request.FullName ?? "Customer"},


Thank you for your interest in the {request.PlanType ?? "Unknown"} plan of MAILMate!

Our team will review your request and provide detailed information about the plan soon.

{(string.IsNullOrEmpty(request.PhoneNumber) ? "" : "Our telesales team may also contact you for a personalized consultation.\n\n")}

Best regards,
The MAILMate Team";

            // Try to load and render template
            try
            {
                var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "ConsultationEmail.html");

                // Check if template file exists
                if (!File.Exists(templatePath))
                {
                    throw new FileNotFoundException($"Template file not found at: {templatePath}");
                }

                // Validate request data
                if (string.IsNullOrEmpty(request.FullName) || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.PlanType))
                {
                    throw new ArgumentException("Required fields (FullName, Email, PlanType) cannot be empty");
                }

                // Render template with RazorLight
                htmlBody = await _razorEngine.CompileRenderAsync("ConsultationEmail.html", request);
            }
            catch (FileNotFoundException ex)
            {
                // Log error and use fallback template
                Console.WriteLine($"Error: {ex.Message}");
            }
            //catch (RazorLightCompilationException ex)
            //{
            //    // Log template compilation error and use fallback
            //    Console.WriteLine($"Template compilation error: {ex.Message}");
            //}
            catch (ArgumentException ex)
            {
                // Log data validation error and use fallback
                Console.WriteLine($"Data validation error: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Log unexpected errors and use fallback
                Console.WriteLine($"Unexpected error while rendering template: {ex.Message}");
            }

            var bodyBuilder = new BodyBuilder
            {
                TextBody = textBody,
            };

            message.Body = bodyBuilder.ToMessageBody();

            try
            {
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(smtpHost, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(smtpUsername, smtpPassword);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {
                // Log SMTP error and throw to be handled by caller
                Console.WriteLine($"SMTP error: {ex.Message}");
                throw new InvalidOperationException("Failed to send consultation email", ex);
            }
        }
    }
}