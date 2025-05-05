using MailMate_BE_V2.DTO;
﻿using MailMate_BE_V2.DTOs;

namespace MailMate_BE_V2.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(string email, string password);
        Task<RegisterResponse> RegisterAsync(RegisterRequest request);
    }
}
