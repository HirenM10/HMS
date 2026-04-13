namespace Clinical.Infrastructure.Options;

public sealed class SeedUserOptions
{
    public const string SectionName = "SeedUsers";

    public IReadOnlyCollection<SeedUserItem> Users { get; init; } = [];
}

public sealed class SeedUserItem
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
}
