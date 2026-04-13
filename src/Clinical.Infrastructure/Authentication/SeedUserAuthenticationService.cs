using BCrypt.Net;
using Clinical.Application.Abstractions.Auth;
using Clinical.Application.Common.Security;
using Clinical.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace Clinical.Infrastructure.Authentication;

public sealed class SeedUserAuthenticationService(IOptions<SeedUserOptions> options) : IUserAuthenticationService
{
    private readonly IReadOnlyCollection<SeedUserItem> _users = options.Value.Users;

    public Task<ApplicationUser?> AuthenticateAsync(string email, string password, CancellationToken cancellationToken)
    {
        var user = _users.SingleOrDefault(x => string.Equals(x.Email, email, StringComparison.OrdinalIgnoreCase));
        if (user is null)
        {
            return Task.FromResult<ApplicationUser?>(null);
        }

        var configuredSecret = string.IsNullOrWhiteSpace(user.SecretEnvironmentVariable)
            ? null
            : Environment.GetEnvironmentVariable(user.SecretEnvironmentVariable);

        if (string.IsNullOrWhiteSpace(configuredSecret) || !BCrypt.Net.BCrypt.Verify(password, BCrypt.Net.BCrypt.HashPassword(configuredSecret)))
        {
            return Task.FromResult<ApplicationUser?>(null);
        }

        return Task.FromResult<ApplicationUser?>(
            new ApplicationUser
            {
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role
            });
    }
}
