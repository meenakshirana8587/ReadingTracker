using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ReadingTracker.Api.Middleware;
using ReadingTracker.Application.Auth;
using ReadingTracker.Domain.Constants;
using ReadingTracker.Domain.Entities;
using ReadingTracker.Infrastructure;
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

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
