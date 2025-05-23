using MailMate_BE_V2.Data;
using MailMate_BE_V2.DTOs.Gemini;
using MailMate_BE_V2.Interfaces;
using MailMate_BE_V2.Models.PayOS;
using MailMate_BE_V2.Services;
using MailMate_BE_V2.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Net.payOS.Constants;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMarketingService, MarketingService>();
builder.Services.AddHttpClient<IEmailAccountService, EmailAccountService>();
builder.Services.AddScoped<EmailUtility>();
builder.Services.AddHttpClient();
builder.Services.AddScoped<IEmailAccountService, EmailAccountService>();
builder.Services.AddScoped<IHuggingFaceService, HuggingFaceService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddHttpClient<IGeminiService, GeminiService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

// Cấu hình Gemini settings
builder.Services.Configure<GeminiSettings>(builder.Configuration.GetSection("Gemini"));

// Đăng ký PayOS configuration
builder.Services.Configure<MailMate_BE_V2.Models.PayOS.PayOSConfig>(builder.Configuration.GetSection("PayOS"));

// Đăng ký service

// Add logging
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
});

// Configure JWT authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
        options.MapInboundClaims = false; // Off mappping claim

    });

builder.Services.AddAuthorization();

// Register DbContext
builder.Services.AddDbContext<MailMateDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// Cấu hình Swagger với khóa Authorize
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MailMate API", Version = "v1" });

    // Thêm nút Authorize trên Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token with: Bearer <token>",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // Yêu cầu tất cả API cần Bearer Token
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});


var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("https://mail-mate-ai-plugin-page.vercel.app", "http://localhost:3000", "https://localhost:3000") // FE domain
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials(); // If use cookie
        });
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("_myAllowSpecificOrigins");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
