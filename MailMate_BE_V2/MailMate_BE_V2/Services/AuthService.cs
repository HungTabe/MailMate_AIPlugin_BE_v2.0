using MailMate_BE_V2.Data;
using MailMate_BE_V2.Data.EnumData;
using MailMate_BE_V2.DTOs;
using MailMate_BE_V2.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace MailMate_BE_V2.Services
{
    public class AuthService : IAuthService
    {
        private readonly MailMateDbContext _context;

        public AuthService(MailMateDbContext context)
        {
            _context = context;
        }

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                throw new Exception("Email already registered.");

            var user = new User
            {
                UserId = Guid.NewGuid(),
                Email = request.Email.Trim().ToLower(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                FullName = request.FullName,
                Role = UserRole.User,  // Gán mặc định
                IsActive = true,
                SubscriptionPlan = SubscriptionPlan.Free,
                SubscriptionEndDate = null,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new RegisterResponse
            {
                UserId = user.UserId,
                Email = user.Email,
                Role = user.Role.ToString(),
                Message = "User registered successfully."
            };
        }
    }
}
