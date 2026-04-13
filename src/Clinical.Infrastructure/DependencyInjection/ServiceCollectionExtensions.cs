using System.Text;
using Clinical.Application.Abstractions.Auth;
using Clinical.Application.Abstractions.Caching;
using Clinical.Application.Abstractions.Data;
using Clinical.Application.Abstractions.Messaging;
using Clinical.Infrastructure.Authentication;
using Clinical.Infrastructure.Caching;
using Clinical.Infrastructure.Data;
using Clinical.Infrastructure.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Clinical.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.Configure<SeedUserOptions>(configuration.GetSection(SeedUserOptions.SectionName));

        var provider = configuration["DatabaseProvider"] ?? "SqlServer";
        var connectionString = configuration.GetConnectionString("ClinicalDb")
            ?? throw new InvalidOperationException("Connection string 'ClinicalDb' is not configured.");

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            if (string.Equals(provider, "Sqlite", StringComparison.OrdinalIgnoreCase))
            {
                options.UseSqlite(connectionString);
                return;
            }

            options.UseSqlServer(connectionString);
        });

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());
        services.AddMemoryCache();
        services.AddSingleton<IQueryCache, MemoryQueryCache>();
        services.AddSingleton<ICacheInvalidator, CacheInvalidator>();
        services.AddSingleton<IUserAuthenticationService, SeedUserAuthenticationService>();
        services.AddSingleton<ITokenService, TokenService>();

        var jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey));

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = signingKey,
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
            });

        services.AddAuthorizationBuilder()
            .AddPolicy("RequireClinicalRead", policy => policy.RequireRole("Administrator", "Doctor", "Receptionist"))
            .AddPolicy("RequireClinicalWrite", policy => policy.RequireRole("Administrator", "Doctor"))
            .AddPolicy("RequireAdmin", policy => policy.RequireRole("Administrator"));

        return services;
    }
}
