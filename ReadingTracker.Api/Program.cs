using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ReadingTracker.Api.Middleware;
using ReadingTracker.Application.Auth;
using ReadingTracker.Application.Services;
using ReadingTracker.Domain.Constants;
using ReadingTracker.Domain.Entities;
using ReadingTracker.Domain.Interfaces;
using ReadingTracker.Infrastructure;
using ReadingTracker.Infrastructure.Repositories;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. Database Context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Identity Core Setup
builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
    options.Password.RequireDigit = UserConstants.RequireDigit;
    options.Password.RequiredLength = UserConstants.MinPasswordLength;
    options.Password.RequireNonAlphanumeric = UserConstants.RequireNonAlphanumeric;
    options.User.RequireUniqueEmail = true;
})
.AddRoles<IdentityRole<Guid>>()
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// 3. JWT Bearer Authentication
var jwtKey = builder.Configuration["Jwt:SecretKey"]!;
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero
    };
});

// 4. Authorization
builder.Services.AddAuthorization(options =>
{
    // Example custom policy
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();

// Add services to the container.
builder.Services.AddScoped<IUserBookRepository, UserBookRepository>();
builder.Services.AddScoped<IReadingSessionRepository, ReadingSessionRepository>();

// Services
builder.Services.AddScoped<IUserBookService, UserBookService>();
builder.Services.AddScoped<IReadingSessionService, ReadingSessionService>();
builder.Services.AddScoped<ReadingTracker.Api.Filters.PerformanceAuditFilter>();
builder.Services.AddHostedService<NightlyCleanupBackgroundService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ReadingTracker API", Version = "v1" });

    // This creates the "Authorize" button and padlock icons
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGci...\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // This tells Swagger to attach the token to every single request
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
            Array.Empty<string>()
        }
    });
});
// Register RFC 7807 Problem Details support
builder.Services.AddProblemDetails();

// Register our custom GlobalExceptionHandler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();

// 1. MUST BE FIRST: Catches unhandled exceptions from any middleware or controller below it
app.UseExceptionHandler();

// 2. Automatically writes ProblemDetails JSON for empty 4xx/5xx responses (like 401 from [Authorize] or 404 on bad URLs)
app.UseStatusCodePages();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UsePerformanceLogging();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
