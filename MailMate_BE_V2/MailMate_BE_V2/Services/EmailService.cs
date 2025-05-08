using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Gmail.v1;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using MailMate_BE_V2.Data;
using MailMate_BE_V2.DTOs;
using MailMate_BE_V2.Interfaces;
using MailMate_BE_V2.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace MailMate_BE_V2.Services
{
    public class EmailService : IEmailService
    {
        private readonly MailMateDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public EmailService(MailMateDbContext dbContext, IConfiguration configuration, HttpClient httpClient)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _httpClient = httpClient;
        }

        //public async Task<List<EmailDto>> GetInboxEmailsAsyncDemo(Guid userId)
        //{
        //    // Find the user's Gmail account
        //    var emailAccount = await _dbContext.EmailAccounts
        //        .FirstOrDefaultAsync(ea => ea.UserId == userId && ea.Provider == "Google");

        //    if (emailAccount == null)
        //    {
        //        throw new Exception("No Gmail account connected.");
        //    }

        //    // Set up OAuth2 client
        //    var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        //    {
        //        ClientSecrets = new ClientSecrets
        //        {
        //            ClientId = _configuration["GoogleOAuth:ClientId"],
        //            ClientSecret = _configuration["GoogleOAuth:ClientSecret"]
        //        },
        //        Scopes = new[] { "https://www.googleapis.com/auth/gmail.readonly" },
        //        DataStore = new NullDataStore() // Không lưu token vào file
        //    });

        //    var token = new Google.Apis.Auth.OAuth2.Responses.TokenResponse
        //    {
        //        AccessToken = emailAccount.AccessToken,
        //        RefreshToken = emailAccount.RefreshToken,
        //        Scope = "https://www.googleapis.com/auth/gmail.readonly",
        //        TokenType = "Bearer",
        //        ExpiresInSeconds = 3600 // Giả định token còn 1 giờ
        //    };

        //    var credential = new UserCredential(flow, "me", token);

        //    // Create Gmail API client
        //    var gmailService = new GmailService(new BaseClientService.Initializer
        //    {
        //        HttpClientInitializer = credential,
        //        ApplicationName = "MailMate",
        //    });

        //    try
        //    {
        //        // Fetch the top 20 emails from the inbox
        //        var request = gmailService.Users.Messages.List("me");
        //        request.Q = "from:inbox";
        //        request.MaxResults = 20;

        //        var messages = await request.ExecuteAsync();
        //        if (messages.Messages == null || !messages.Messages.Any())
        //        {
        //            return new List<EmailDto>();
        //        }

        //        var emails = new List<EmailDto>();
        //        foreach (var message in messages.Messages)
        //        {
        //            var email = await gmailService.Users.Messages.Get("me", message.Id).ExecuteAsync();
        //            var headers = email.Payload.Headers;
        //            var subject = headers.FirstOrDefault(h => h.Name.ToLower() == "subject")?.Value ?? "No Subject";
        //            var from = headers.FirstOrDefault(h => h.Name.ToLower() == "from")?.Value ?? "Unknown Sender";

        //            // Extract email body
        //            string body = "";
        //            if (email.Payload.Parts != null)
        //            {
        //                var textPart = email.Payload.Parts.FirstOrDefault(p => p.MimeType == "text/plain");
        //                if (textPart?.Body?.Data != null)
        //                {
        //                    try
        //                    {
        //                        byte[] decodedBytes = Convert.FromBase64String(textPart.Body.Data.Replace('-', '+').Replace('_', '/'));
        //                        body = Encoding.UTF8.GetString(decodedBytes);
        //                    }
        //                    catch (FormatException)
        //                    {
        //                        body = "Unable to decode email body.";
        //                    }
        //                }
        //            }
        //            else if (email.Payload.Body?.Data != null)
        //            {
        //                try
        //                {
        //                    byte[] decodedBytes = Convert.FromBase64String(email.Payload.Body.Data.Replace('-', '+').Replace('_', '/'));
        //                    body = Encoding.UTF8.GetString(decodedBytes);
        //                }
        //                catch (FormatException)
        //                {
        //                    body = "Unable to decode email body.";
        //                }
        //            }

        //            // Generate a simple summary
        //            var summary = body.Length > 200 ? body.Substring(0, 200) + "..." : body;

        //            // Check for duplicate email
        //            var existingEmail = await _dbContext.Emails
        //                .FirstOrDefaultAsync(e => e.EmailAccountId == emailAccount.EmailAccountId && e.EmailId == email.Id);

        //            if (existingEmail == null)
        //            {
        //                // Parse InternalDate safely
        //                DateTime receivedAt = DateTime.UtcNow;
        //                //if (!string.IsNullOrEmpty(email.InternalDate))
        //                //{
        //                //    if (long.TryParse(email.InternalDate, out long internalDate))
        //                //    {
        //                //        try
        //                //        {
        //                //            receivedAt = DateTimeOffset.FromUnixTimeMilliseconds(internalDate).UtcDateTime;
        //                //        }
        //                //        catch (ArgumentOutOfRangeException)
        //                //        {
        //                //            receivedAt = DateTime.UtcNow;
        //                //        }
        //                //    }
        //                //}

        //                // Save to database
        //                var newEmail = new Email
        //                {
        //                    EmailId = email.Id, // email.Id is string
        //                    EmailAccountId = emailAccount.EmailAccountId,
        //                    Subject = subject,
        //                    Body = body,
        //                    Summary = summary,
        //                    IsSpam = email.LabelIds?.Contains("SPAM") ?? false,
        //                    ReceivedAt = receivedAt
        //                };
        //                _dbContext.Emails.Add(newEmail);
        //                await _dbContext.SaveChangesAsync();

        //                emails.Add(new EmailDto
        //                {
        //                    EmailId = newEmail.EmailId,
        //                    Subject = newEmail.Subject,
        //                    Summary = newEmail.Summary,
        //                    IsSpam = newEmail.IsSpam,
        //                    ReceivedAt = newEmail.ReceivedAt,
        //                    From = from
        //                });
        //            }
        //            else
        //            {
        //                emails.Add(new EmailDto
        //                {
        //                    EmailId = existingEmail.EmailId,
        //                    Subject = existingEmail.Subject,
        //                    Summary = existingEmail.Summary,
        //                    IsSpam = existingEmail.IsSpam,
        //                    ReceivedAt = existingEmail.ReceivedAt,
        //                    From = from
        //                });
        //            }
        //        }

        //        // Log the action
        //        _dbContext.Logs.Add(new Log
        //        {
        //            LogId = Guid.NewGuid(),
        //            UserId = userId,
        //            Action = "Fetched inbox emails",
        //            Timestamp = DateTime.UtcNow,
        //            Metadata = System.Text.Json.JsonSerializer.Serialize(new { EmailCount = emails.Count })
        //        });
        //        await _dbContext.SaveChangesAsync();

        //        return emails;
        //    }
        //    catch (Google.GoogleApiException ex) when (ex.HttpStatusCode == System.Net.HttpStatusCode.Unauthorized)
        //    {
        //        // Refresh token and retry
        //        await RefreshAccessTokenAsync(emailAccount);
        //        return await GetInboxEmailsAsync(userId); // Retry
        //    }
        //}

        public async Task<List<EmailDto>> GetInboxEmailsAsync(Guid userId)
        {
            // Find the user's Gmail account
            var emailAccount = await _dbContext.EmailAccounts
                .FirstOrDefaultAsync(ea => ea.UserId == userId && ea.Provider == "Google");

            if (emailAccount == null)
            {
                throw new Exception("No Gmail account connected.");
            }

            // Set up OAuth2 client
            var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = _configuration["GoogleOAuth:ClientId"],
                    ClientSecret = _configuration["GoogleOAuth:ClientSecret"]
                },
                Scopes = new[] { "https://www.googleapis.com/auth/gmail.readonly" },
                DataStore = new NullDataStore()
            });

            var token = new Google.Apis.Auth.OAuth2.Responses.TokenResponse
            {
                AccessToken = emailAccount.AccessToken,
                RefreshToken = emailAccount.RefreshToken,
                Scope = "https://www.googleapis.com/auth/gmail.readonly",
                TokenType = "Bearer",
                ExpiresInSeconds = 3600
            };

            var credential = new UserCredential(flow, "me", token);

            // Create Gmail API client
            var gmailService = new GmailService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "MailMate",
            });

            try
            {
                // Fetch the top 20 emails from the inbox
                var request = gmailService.Users.Messages.List("me");
                request.Q = "in:inbox"; // Sửa từ "from:inbox" thành "in:inbox"
                request.MaxResults = 20;

                var messages = await request.ExecuteAsync();
                if (messages.Messages == null || !messages.Messages.Any())
                {
                    Console.WriteLine($"No messages found for user {userId}. Query: {request.Q}");
                    return new List<EmailDto>();
                }

                var emails = new List<EmailDto>();
                foreach (var message in messages.Messages)
                {
                    var email = await gmailService.Users.Messages.Get("me", message.Id).ExecuteAsync();
                    var headers = email.Payload.Headers;
                    var subject = headers.FirstOrDefault(h => h.Name.ToLower() == "subject")?.Value ?? "No Subject";
                    var from = headers.FirstOrDefault(h => h.Name.ToLower() == "from")?.Value ?? "Unknown Sender";

                    // Extract email body
                    string body = "";
                    if (email.Payload.Parts != null)
                    {
                        var textPart = email.Payload.Parts.FirstOrDefault(p => p.MimeType == "text/plain");
                        if (textPart?.Body?.Data != null)
                        {
                            try
                            {
                                byte[] decodedBytes = Convert.FromBase64String(textPart.Body.Data.Replace('-', '+').Replace('_', '/'));
                                body = Encoding.UTF8.GetString(decodedBytes);
                            }
                            catch (FormatException)
                            {
                                body = "Unable to decode email body.";
                            }
                        }
                    }
                    else if (email.Payload.Body?.Data != null)
                    {
                        try
                        {
                            byte[] decodedBytes = Convert.FromBase64String(email.Payload.Body.Data.Replace('-', '+').Replace('_', '/'));
                            body = Encoding.UTF8.GetString(decodedBytes);
                        }
                        catch (FormatException)
                        {
                            body = "Unable to decode email body.";
                        }
                    }

                    // Generate a simple summary
                    var summary = body.Length > 200 ? body.Substring(0, 200) + "..." : body;

                    // Check for duplicate email
                    var existingEmail = await _dbContext.Emails
                        .FirstOrDefaultAsync(e => e.EmailAccountId == emailAccount.EmailAccountId && e.EmailId == email.Id);

                    if (existingEmail == null)
                    {
                        // Parse InternalDate safely
                        DateTime receivedAt = DateTime.UtcNow;
                        //if (!string.IsNullOrEmpty(email.InternalDate))
                        //{
                        //    if (long.TryParse(email.InternalDate, out long internalDate))
                        //    {
                        //        try
                        //        {
                        //            receivedAt = DateTimeOffset.FromUnixTimeMilliseconds(internalDate).UtcDateTime;
                        //        }
                        //        catch (ArgumentOutOfRangeException)
                        //        {
                        //            receivedAt = DateTime.UtcNow;
                        //        }
                        //    }
                        //}

                        // Save to database
                        var newEmail = new Email
                        {
                            EmailId = email.Id,
                            EmailAccountId = emailAccount.EmailAccountId,
                            Subject = subject,
                            Body = body,
                            Summary = summary,
                            IsSpam = email.LabelIds?.Contains("SPAM") ?? false,
                            ReceivedAt = receivedAt
                        };
                        _dbContext.Emails.Add(newEmail);
                        await _dbContext.SaveChangesAsync();

                        emails.Add(new EmailDto
                        {
                            EmailId = newEmail.EmailId,
                            Subject = newEmail.Subject,
                            Summary = newEmail.Summary,
                            IsSpam = newEmail.IsSpam,
                            ReceivedAt = newEmail.ReceivedAt,
                            From = from
                        });
                    }
                    else
                    {
                        emails.Add(new EmailDto
                        {
                            EmailId = existingEmail.EmailId,
                            Subject = existingEmail.Subject,
                            Summary = existingEmail.Summary,
                            IsSpam = existingEmail.IsSpam,
                            ReceivedAt = existingEmail.ReceivedAt,
                            From = from
                        });
                    }
                }

                // Log the action
                _dbContext.Logs.Add(new Log
                {
                    LogId = Guid.NewGuid(),
                    UserId = userId,
                    Action = "Fetched inbox emails",
                    Timestamp = DateTime.UtcNow,
                    Metadata = System.Text.Json.JsonSerializer.Serialize(new { EmailCount = emails.Count })
                });
                await _dbContext.SaveChangesAsync();

                return emails;
            }
            catch (Google.GoogleApiException ex)
            {
                Console.WriteLine($"Gmail API error: {ex.HttpStatusCode}, {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                throw;
            }
        }

        private async Task RefreshAccessTokenAsync(EmailAccount emailAccount)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://oauth2.googleapis.com/token");
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "client_id", _configuration["Google:ClientId"] },
                { "client_secret", _configuration["Google:ClientSecret"] },
                { "refresh_token", emailAccount.RefreshToken },
                { "grant_type", "refresh_token" }
            });
            request.Content = content;

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to refresh access token.");
            }

            var json = await response.Content.ReadAsStringAsync();
            var tokenResponse = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            emailAccount.AccessToken = tokenResponse["access_token"];
            await _dbContext.SaveChangesAsync();
        }
    }
}
