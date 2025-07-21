using System.Text.RegularExpressions;

namespace Tsikhanau.Outcomes.Validation;

public static class ValidationExtensions
{
    public static PropertyValidator<T, TProperty?> NotNull<T, TProperty>(this PropertyValidator<T, TProperty?> validator, String? errorMessage = null)
        where TProperty : class
    {
        return validator.Must(value => value is not null, errorMessage ?? "Value cannot be null");
    }

    public static PropertyValidator<T, TProperty?> NotNull<T, TProperty>(this PropertyValidator<T, TProperty?> validator, String? errorMessage = null)
        where TProperty : struct
    {
        return validator.Must(value => value.HasValue, errorMessage ?? "Value cannot be null");
    }

    public static PropertyValidator<T, String?> NotEmpty<T>(this PropertyValidator<T, String?> validator, String? errorMessage = null)
    {
        return validator.Must(value => !String.IsNullOrEmpty(value), errorMessage ?? "Value cannot be empty");
    }

    public static PropertyValidator<T, String?> NotWhiteSpace<T>(this PropertyValidator<T, String?> validator, String? errorMessage = null)
    {
        return validator.Must(value => !String.IsNullOrWhiteSpace(value), errorMessage ?? "Value cannot be whitespace");
    }

    public static PropertyValidator<T, String?> MinLength<T>(this PropertyValidator<T, String?> validator, Int32 minLength, String? errorMessage = null)
    {
        return validator.Must(
            value => value?.Length >= minLength, 
            errorMessage ?? $"Value must be at least {minLength} characters long");
    }

    public static PropertyValidator<T, String?> MaxLength<T>(this PropertyValidator<T, String?> validator, Int32 maxLength, String? errorMessage = null)
    {
        return validator.Must(
            value => value?.Length <= maxLength, 
            errorMessage ?? $"Value must be at most {maxLength} characters long");
    }

    public static PropertyValidator<T, String?> Email<T>(this PropertyValidator<T, String?> validator, String? errorMessage = null)
    {
        const String emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return validator.Must(
            value => value != null && Regex.IsMatch(value, emailPattern),
            errorMessage ?? "Value must be a valid email address");
    }

    public static PropertyValidator<T, String?> MatchesRegex<T>(this PropertyValidator<T, String?> validator, String pattern, String? errorMessage = null)
    {
        return validator.Must(
            value => value != null && Regex.IsMatch(value, pattern),
            errorMessage ?? $"Value must match pattern: {pattern}");
    }

    public static PropertyValidator<T, TProperty> GreaterThan<T, TProperty>(this PropertyValidator<T, TProperty> validator, TProperty threshold, String? errorMessage = null)
        where TProperty : IComparable<TProperty>
    {
        return validator.Must(
            value => value.CompareTo(threshold) > 0,
            errorMessage ?? $"Value must be greater than {threshold}");
    }

    public static PropertyValidator<T, TProperty> GreaterThanOrEqual<T, TProperty>(this PropertyValidator<T, TProperty> validator, TProperty threshold, String? errorMessage = null)
        where TProperty : IComparable<TProperty>
    {
        return validator.Must(
            value => value.CompareTo(threshold) >= 0,
            errorMessage ?? $"Value must be greater than or equal to {threshold}");
    }

    public static PropertyValidator<T, TProperty> LessThan<T, TProperty>(this PropertyValidator<T, TProperty> validator, TProperty threshold, String? errorMessage = null)
        where TProperty : IComparable<TProperty>
    {
        return validator.Must(
            value => value.CompareTo(threshold) < 0,
            errorMessage ?? $"Value must be less than {threshold}");
    }

    public static PropertyValidator<T, TProperty> LessThanOrEqual<T, TProperty>(this PropertyValidator<T, TProperty> validator, TProperty threshold, String? errorMessage = null)
        where TProperty : IComparable<TProperty>
    {
        return validator.Must(
            value => value.CompareTo(threshold) <= 0,
            errorMessage ?? $"Value must be less than or equal to {threshold}");
    }

    public static PropertyValidator<T, TProperty> Equal<T, TProperty>(this PropertyValidator<T, TProperty> validator, TProperty expected, String? errorMessage = null)
        where TProperty : IEquatable<TProperty>
    {
        return validator.Must(
            value => value.Equals(expected),
            errorMessage ?? $"Value must equal {expected}");
    }

    public static PropertyValidator<T, TProperty> NotEqual<T, TProperty>(this PropertyValidator<T, TProperty> validator, TProperty forbidden, String? errorMessage = null)
        where TProperty : IEquatable<TProperty>
    {
        return validator.Must(
            value => !value.Equals(forbidden),
            errorMessage ?? $"Value must not equal {forbidden}");
    }

    public static PropertyValidator<T, TProperty> In<T, TProperty>(this PropertyValidator<T, TProperty> validator, IEnumerable<TProperty> allowedValues, String? errorMessage = null)
        where TProperty : IEquatable<TProperty>
    {
        var allowed = allowedValues.ToList();
        return validator.Must(
            value => allowed.Contains(value),
            errorMessage ?? $"Value must be one of: {String.Join(", ", allowed)}");
    }

    public static PropertyValidator<T, TProperty> NotIn<T, TProperty>(this PropertyValidator<T, TProperty> validator, IEnumerable<TProperty> forbiddenValues, String? errorMessage = null)
        where TProperty : IEquatable<TProperty>
    {
        var forbidden = forbiddenValues.ToList();
        return validator.Must(
            value => !forbidden.Contains(value),
            errorMessage ?? $"Value must not be one of: {String.Join(", ", forbidden)}");
    }

    public static PropertyValidator<T, IEnumerable<TItem>> NotEmpty<T, TItem>(this PropertyValidator<T, IEnumerable<TItem>> validator, String? errorMessage = null)
    {
        return validator.Must(
            value => value?.Any() == true,
            errorMessage ?? "Collection cannot be empty");
    }

    public static PropertyValidator<T, IEnumerable<TItem>> Count<T, TItem>(this PropertyValidator<T, IEnumerable<TItem>> validator, Int32 expectedCount, String? errorMessage = null)
    {
        return validator.Must(
            value => value?.Count() == expectedCount,
            errorMessage ?? $"Collection must contain exactly {expectedCount} items");
    }

    public static PropertyValidator<T, IEnumerable<TItem>> MinCount<T, TItem>(this PropertyValidator<T, IEnumerable<TItem>> validator, Int32 minCount, String? errorMessage = null)
    {
        return validator.Must(
            value => value?.Count() >= minCount,
            errorMessage ?? $"Collection must contain at least {minCount} items");
    }

    public static PropertyValidator<T, IEnumerable<TItem>> MaxCount<T, TItem>(this PropertyValidator<T, IEnumerable<TItem>> validator, Int32 maxCount, String? errorMessage = null)
    {
        return validator.Must(
            value => value?.Count() <= maxCount,
            errorMessage ?? $"Collection must contain at most {maxCount} items");
    }
}