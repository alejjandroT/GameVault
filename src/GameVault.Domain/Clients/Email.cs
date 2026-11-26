using System.Text.RegularExpressions;
using GameVault.Domain.Common;

namespace GameVault.Domain.Clients;

public sealed record Email
{
    private static readonly Regex Format = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private Email(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<Email> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<Email>(ClientErrors.EmailRequired);

        var normalized = value.Trim().ToLowerInvariant();
        if (!Format.IsMatch(normalized))
            return Result.Failure<Email>(ClientErrors.EmailInvalid);

        return Result.Success(new Email(normalized));
    }

    public static Email Rehydrate(string value) => new(value);

    public override string ToString() => Value;
}