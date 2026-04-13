using Clinical.Application.Common.Security;

namespace Clinical.Application.Abstractions.Auth;

public interface IUserAuthenticationService
{
    Task<ApplicationUser?> AuthenticateAsync(string email, string password, CancellationToken cancellationToken);
}
