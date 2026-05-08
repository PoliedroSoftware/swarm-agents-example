using System.Text.RegularExpressions;
using SwarmDemo.Domain.Common;

namespace SwarmDemo.Domain.Products.ValueObjects;

public sealed partial class Sku : ValueObject
{
    [GeneratedRegex("^[A-Z0-9-]+$")]
    private static partial Regex AllowedPattern();

    public string Value { get; }

    private Sku(string value) => Value = value;

    public static Sku Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("SKU cannot be empty.", nameof(value));
        if (value.Length > 32)
            throw new ArgumentException("SKU cannot exceed 32 characters.", nameof(value));
        if (!AllowedPattern().IsMatch(value))
            throw new ArgumentException("SKU must contain only uppercase letters, digits and hyphens.", nameof(value));

        return new Sku(value);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
