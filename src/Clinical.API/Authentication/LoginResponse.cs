namespace Clinical.API.Authentication;

public sealed record LoginResponse(string AccessToken, string TokenType, string Role, string Email, string FullName);
