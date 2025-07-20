using System.Collections;
using System.Text.RegularExpressions;

namespace Tsikhanau.Foundation.Validation;

public sealed class Guard<T>
{
    private readonly String _paramName;

    private Guard(T value, String paramName)
    {
        Value = value;
        _paramName = paramName;
    }

    public static Guard<T> For(T value, String paramName = "")
    {
        return new Guard<T>(value, paramName);
    }

    public Guard<T> NotNull()
    {
        Guard.AgainstNull(Value);
        return this;
    }

    public Guard<T> NotEmpty()
    {
        switch (Value)
        {
            case String str:
                Guard.AgainstNullOrEmpty(str);
                break;
            case ICollection collection:
                Guard.AgainstNullOrEmpty(collection);
                break;
            default:
                throw new InvalidOperationException($"NotEmpty() can only be used with string or ICollection types, but was called on {typeof(T).Name}.");
        }
        return this;
    }

    public Guard<T> Matches(Regex regex)
    {
        if (Value is not String str)
            throw new InvalidOperationException($"Matches() can only be used with string types, but was called on {typeof(T).Name}.");

        Guard.AgainstNull(regex);
        
        if (!regex.IsMatch(str))
            Guard.Against(true, $"Value does not match the required pattern.", _paramName);

        return this;
    }

    public Guard<T> MaxLength(Int32 maxLength)
    {
        if (Value is not String str)
            throw new InvalidOperationException($"MaxLength() can only be used with string types, but was called on {typeof(T).Name}.");

        Guard.Against(maxLength < 0, "Maximum length cannot be negative.", "maxLength");
        Guard.Against(str.Length > maxLength, $"String length ({str.Length}) exceeds maximum length ({maxLength}).", _paramName);

        return this;
    }

    public Guard<T> MinLength(Int32 minLength)
    {
        if (Value is not String str)
            throw new InvalidOperationException($"MinLength() can only be used with string types, but was called on {typeof(T).Name}.");

        Guard.Against(minLength < 0, "Minimum length cannot be negative.", "minLength");
        Guard.Against(str.Length < minLength, $"String length ({str.Length}) is less than minimum length ({minLength}).", _paramName);

        return this;
    }

    public Guard<T> InRange<TComparable>(TComparable min, TComparable max)
        where TComparable : IComparable<TComparable>
    {
        if (Value is not TComparable comparableValue)
            throw new InvalidOperationException($"InRange() can only be used with IComparable types, but was called on {typeof(T).Name}.");

        Guard.AgainstOutOfRange(comparableValue, min, max);
        return this;
    }

    private T Value { get; }

    public static implicit operator T(Guard<T> guard) => guard.Value;
}