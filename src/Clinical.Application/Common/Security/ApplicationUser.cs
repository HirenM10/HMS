namespace Clinical.Application.Common.Security;

public sealed class ApplicationUser
{
    public required string Email { get; init; }
    public required string Role { get; init; }
    public required string FullName { get; init; }
}
