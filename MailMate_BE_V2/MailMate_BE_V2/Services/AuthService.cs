using MailMate_BE_V2.Data;
using MailMate_BE_V2.Data.EnumData;
using MailMate_BE_V2.DTO;
using MailMate_BE_V2.DTOs;
using MailMate_BE_V2.Interfaces;
using MailMate_BE_V2.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MailMate_BE_V2.Services
{
    public class AuthService : IAuthService
    {
        private readonly MailMateDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(MailMateDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<LoginResponse> LoginAsync(string email, string password)
        {
            // Find user by email
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

            if (user == null || !user.IsActive)
            {
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            // Generate JWT token
            var token = GenerateJwtToken(user);

            return new LoginResponse
            {
                Token = token,
                ExpiresIn = 3600, // 1 hour
                User = new UserDto
                {
                    UserId = user.UserId,
                    Email = user.Email,
                    FullName = user.FullName,
                    Role = user.Role.ToString()
                }
            };
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
                Role = UserRole.User,
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

        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<UserProfileDto> GetUserProfileAsync(string userId)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == Guid.Parse(userId));

            if (user == null)
            {
                throw new Exception("User not found.");
            }
            // Lấy Provider từ bảng EmailAccounts (lấy bản ghi đầu tiên nếu có nhiều)
            var emailAccount = await _context.EmailAccounts
                .Where(ea => ea.UserId == user.UserId)
                .FirstOrDefaultAsync();

            return new UserProfileDto
            {
                UserId = user.UserId,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role,
                IsActive = user.IsActive,
                SubscriptionPlan = user.SubscriptionPlan,
                SubscriptionEndDate = user.SubscriptionEndDate,
                CreatedAt = user.CreatedAt,
                Provider = emailAccount?.Provider // Lấy Provider, nếu không có thì để null
            };
        }
    }
}
