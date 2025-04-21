using MailKit.Net.Smtp;
using MailMate_BE_V2.DTOs;
using MimeKit;

namespace MailMate_BE_V2.Utilities
{
    public class EmailUtility
    {
        private readonly IConfiguration _configuration;

        public EmailUtility(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendConsultationEmailAsync(ConsultationRequest request)
        {
            var emailConfig = _configuration.GetSection("EmailSettings");
            var senderEmail = emailConfig["SenderEmail"];
            var senderName = emailConfig["SenderName"];
            var smtpHost = emailConfig["SmtpHost"];
            var smtpPort = int.Parse(emailConfig["SmtpPort"]);
            var smtpUsername = emailConfig["SmtpUsername"];
            var smtpPassword = emailConfig["SmtpPassword"];

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(senderName, senderEmail));
            message.To.Add(new MailboxAddress(request.FullName, request.Email));
            message.Subject = $"MAILMate Consultation for {request.PlanType} Plan";

            var bodyBuilder = new BodyBuilder
            {
                TextBody = $@"Dear {request.FullName},

                Thank you for your interest in the {request.PlanType} plan of MAILMate! Our team will review your request and provide detailed information about the plan soon.

                {(string.IsNullOrEmpty(request.PhoneNumber) ? "" : "Our telesales team may also contact you at " + request.PhoneNumber + " for a personalized consultation.")}

                Best regards,
                The MAILMate Team"
            };

            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(smtpHost, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(smtpUsername, smtpPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
         }
    }
}
