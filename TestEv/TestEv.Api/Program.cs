using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TestEv.Application;
using DomainExceptions = TestEv.Domain.Exceptions;
using TestEv.Infrastructure;
using TestEv.Infrastructure.Configuration;
using TestEv.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Projects API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

var jwtSettings = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
    ?? throw new InvalidOperationException("JWT settings are not configured.");

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
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

if (builder.Configuration.GetValue<bool>("Features:EnableBackgroundWorkers"))
{
    builder.Services.AddBackgroundWorkers();
}

builder.Services.AddHealthChecks();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetService<DynamoDbTableInitializer>();
    if (initializer != null)
    {
        await initializer.InitializeAsync();
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Projects API v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exception = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;
        context.Response.ContentType = "application/json";

        var (statusCode, response) = exception switch
        {
            DomainExceptions.EntityNotFoundException ex => (StatusCodes.Status404NotFound,
                new { error = ex.Code, message = ex.Message }),
            DomainExceptions.ValidationException ex => (StatusCodes.Status400BadRequest,
                (object)new { error = ex.Code, message = ex.Message, errors = ex.Errors }),
            DomainExceptions.AuthenticationException ex => (StatusCodes.Status401Unauthorized,
                new { error = ex.Code, message = ex.Message }),
            DomainExceptions.AuthorizationException ex => (StatusCodes.Status403Forbidden,
                new { error = ex.Code, message = ex.Message }),
            _ => (StatusCodes.Status500InternalServerError,
                (object)new { error = "INTERNAL_ERROR", message = "An unexpected error occurred" })
        };

        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(response);
    });
});

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health/live");

app.Run();

public partial class Program { }
