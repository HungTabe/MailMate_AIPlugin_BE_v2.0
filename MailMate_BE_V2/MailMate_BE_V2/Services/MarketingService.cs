using MailKit.Net.Smtp;
using MailMate_BE_V2.Data;
using MailMate_BE_V2.Data.EnumData;
using MailMate_BE_V2.DTOs;
using MailMate_BE_V2.Interfaces;
using MailMate_BE_V2.Models;
using MailMate_BE_V2.Utilities;
using MimeKit;
using System.Text.RegularExpressions;

namespace MailMate_BE_V2.Services
{
    public class MarketingService : IMarketingService
    {
        private readonly MailMateDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly EmailUtility _emailUtility;


        public MarketingService(MailMateDbContext context, IConfiguration configuration, EmailUtility emailUtility)
        {
            _context = context;
            _configuration = configuration;
            _emailUtility = emailUtility;

        }

        public async Task SubmitConsultationRequestAsync(ConsultationRequest request)
        {
            // Validate PlanType
            if (!Enum.TryParse<SubscriptionPlan>(request.PlanType, true, out var planType) ||
                !Enum.IsDefined(typeof(SubscriptionPlan), planType))
            {
                throw new ArgumentException("Invalid plan type. Must be Free, Pro, or Business.");
            }

            // Validate PhoneNumber format (if provided)
            if (!string.IsNullOrEmpty(request.PhoneNumber))
            {
                var phoneRegex = new Regex(@"^\+?[1-9]\d{1,14}$");
                if (!phoneRegex.IsMatch(request.PhoneNumber))
                {
                    throw new ArgumentException("Invalid phone number format.");
                }
            }

            // Create MarketingLead
            var lead = new MarketingLead
            {
                LeadId = Guid.NewGuid(),
                FullName = request.FullName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                PlanType = planType,
                CreatedAt = DateTime.UtcNow,
                Status = LeadStatus.Pending
            };

            // Save to database
            _context.MarketingLeads.Add(lead);
            await _context.SaveChangesAsync();

            // Send consultation email
            await _emailUtility.SendConsultationEmailAsync(request);
        }
    }
}
