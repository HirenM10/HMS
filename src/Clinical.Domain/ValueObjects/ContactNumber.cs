using System.Text.RegularExpressions;

namespace Clinical.Domain.ValueObjects;

public sealed partial record ContactNumber
{
    private ContactNumber(string value) => Value = value;

    public string Value { get; }

    public static ContactNumber Create(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var normalized = value.Trim();

        if (!PhoneRegex().IsMatch(normalized))
        {
            throw new ArgumentException("Contact number format is invalid.", nameof(value));
        }

        return new ContactNumber(normalized);
    }

    [GeneratedRegex(@"^\+?[0-9\s\-]{7,20}$", RegexOptions.Compiled)]
    private static partial Regex PhoneRegex();

    public override string ToString() => Value;
}
