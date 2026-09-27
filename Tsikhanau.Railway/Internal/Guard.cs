using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Tsikhanau.Railway;

internal static class Guard
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T AgainstNull<T>([NotNull] T? value, [CallerArgumentExpression(nameof(value))] String paramName = "")
    {
        if (value is null)
            throw new ArgumentNullException(paramName);
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Against(Boolean condition, String message, String? paramName = null)
    {
        if (condition)
            throw new ArgumentException(message, paramName);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AgainstOutOfRange<T>(T value, T min, T max, [CallerArgumentExpression(nameof(value))] String paramName = "")
        where T : IComparable<T>
    {
        if (value.CompareTo(min) < 0 || value.CompareTo(max) > 0)
            throw new ArgumentOutOfRangeException(paramName, value, $"Value must be between {min} and {max}.");
    }
    
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T AgainstNullOrEmpty<T>([NotNull] T? value, [CallerArgumentExpression(nameof(value))] String paramName = "")
        where T : ICollection
    {
        if (value is null)
            throw new ArgumentNullException(paramName);
        if (value.Count == 0)
            throw new ArgumentException("Collection cannot be empty.", paramName);
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static String AgainstNullOrEmpty([NotNull] String? value, [CallerArgumentExpression(nameof(value))] String paramName = "")
    {
        if (String.IsNullOrEmpty(value))
            throw new ArgumentException("Value cannot be null or empty.", paramName);
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static String AgainstNullOrWhiteSpace([NotNull] String? value, [CallerArgumentExpression(nameof(value))] String paramName = "")
    {
        if (String.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value cannot be null, empty, or whitespace.", paramName);
        return value;
    }
}