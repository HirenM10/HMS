using Clinical.Application.Common.Security;

namespace Clinical.Application.Abstractions.Auth;

public interface ITokenService
{
    string CreateToken(ApplicationUser user);
}
