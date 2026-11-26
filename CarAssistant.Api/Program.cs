using System.Text;
using CarAssistant.Api.Configuration;
using CarAssistant.Api.Data;
using CarAssistant.Api.Entities;
using CarAssistant.Api.Repositories;
using CarAssistant.Api.Repositories.Interfaces;
using CarAssistant.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Options;
using OpenAI;

var builder = WebApplication.CreateBuilder(args);

// 1) Bind JwtSettings from appsettings
var jwtSettings = new JwtSettings();
builder.Configuration.GetSection("Jwt").Bind(jwtSettings);
// Make JwtSettings available as a singleton for configuration access
builder.Services.AddSingleton(jwtSettings);
builder.Services.Configure<OpenAiSettings>(builder.Configuration.GetSection("OpenAi"));
builder.Services.AddHttpClient<CarAssistantService>(); // we'll create this service next

builder.Services.AddSingleton(sp =>
{
    var settings = sp.GetRequiredService<IOptions<OpenAiSettings>>().Value;
    return new OpenAIClient(settings.ApiKey);
});
// 2) DbContext with SQLite
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(connectionString);
});

// 3) Password hasher
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

// 4) JWT Auth Configuration
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key));

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
        
        // This is crucial for debugging: The logging events show why auth failed.
        // I have kept them simple here, but uncomment the OnAuthenticationFailed
        // if you need more explicit error logging in your console.
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                // Log the precise reason for failure (e.g., expired token, invalid key)
                Console.WriteLine($"JWT AUTH FAILED: {context.Exception.Message}");
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

// 5) Repositories & Services
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICarModelRepository, CarModelRepository>();
builder.Services.AddScoped<ICarRepository, CarRepository>();
builder.Services.AddScoped<IManualRepository, ManualRepository>();
builder.Services.AddScoped<ITelemetryRepository, TelemetryRepository>();



// Services
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<CarModelService>();
builder.Services.AddScoped<CarService>();
builder.Services.AddScoped<JwtTokenService>();
builder.Services.AddScoped<ManualService>(); // 👈 add this
builder.Services.AddScoped<CarHealthService>();
builder.Services.AddScoped<TelemetryIngestionService>();
builder.Services.AddScoped<CarAssistantService>();


// Configuration service (simplified, as JwtSettings is now directly injected)
builder.Services.AddSingleton<IAppConfigurationService, AppConfigurationService>();


// 6) Swagger + JWT Setup
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CarAssistant API",
        Version = "v1"
    });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer {token}'",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",           // Mandatory: lowercase "bearer"
        BearerFormat = "JWT"
    };

    // Defines the scheme named "Bearer"
    c.AddSecurityDefinition("Bearer", securityScheme);

    // Applies the security scheme globally (or to any method tagged with [Authorize])
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

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// IMPORTANT: Authentication MUST come before Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();