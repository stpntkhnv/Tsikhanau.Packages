using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Tsikhanau.Guards;

/// <summary>
/// Provides comprehensive guard clauses for method parameter validation.
/// Guards help ensure that method contracts are satisfied by throwing appropriate exceptions
/// when invalid arguments are passed.
/// </summary>
public static class Guard
{
    #region Null Guards

    /// <summary>
    /// Guards against null values.
    /// </summary>
    /// <typeparam name="T">The type of the value being guarded.</typeparam>
    /// <param name="value">The value to guard against null.</param>
    /// <param name="paramName">The name of the parameter (automatically captured).</param>
    /// <returns>The non-null value.</returns>
    /// <exception cref="ArgumentNullException">Thrown when value is null.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T AgainstNull<T>([NotNull] T? value, [CallerArgumentExpression(nameof(value))] string paramName = "")
    {
        if (value is null)
            throw new ArgumentNullException(paramName, $"Parameter '{paramName}' cannot be null.");
        return value;
    }

    /// <summary>
    /// Guards against null values with a custom message.
    /// </summary>
    /// <typeparam name="T">The type of the value being guarded.</typeparam>
    /// <param name="value">The value to guard against null.</param>
    /// <param name="message">Custom error message.</param>
    /// <param name="paramName">The name of the parameter (automatically captured).</param>
    /// <returns>The non-null value.</returns>
    /// <exception cref="ArgumentNullException">Thrown when value is null.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T AgainstNull<T>([NotNull] T? value, string message, [CallerArgumentExpression(nameof(value))] string paramName = "")
    {
        if (value is null)
            throw new ArgumentNullException(paramName, message);
        return value;
    }

    #endregion

    #region String Guards

    /// <summary>
    /// Guards against null or empty strings.
    /// </summary>
    /// <param name="value">The string value to guard.</param>
    /// <param name="paramName">The name of the parameter (automatically captured).</param>
    /// <returns>The non-null, non-empty string.</returns>
    /// <exception cref="ArgumentException">Thrown when value is null or empty.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string AgainstNullOrEmpty([NotNull] string? value, [CallerArgumentExpression(nameof(value))] string paramName = "")
    {
        if (string.IsNullOrEmpty(value))
            throw new ArgumentException($"Parameter '{paramName}' cannot be null or empty.", paramName);
        return value;
    }

    /// <summary>
    /// Guards against null, empty, or whitespace-only strings.
    /// </summary>
    /// <param name="value">The string value to guard.</param>
    /// <param name="paramName">The name of the parameter (automatically captured).</param>
    /// <returns>The non-null, non-empty, non-whitespace string.</returns>
    /// <exception cref="ArgumentException">Thrown when value is null, empty, or whitespace.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string AgainstNullOrWhiteSpace([NotNull] string? value, [CallerArgumentExpression(nameof(value))] string paramName = "")
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"Parameter '{paramName}' cannot be null, empty, or whitespace.", paramName);
        return value;
    }

    /// <summary>
    /// Guards against strings that exceed a maximum length.
    /// </summary>
    /// <param name="value">The string value to guard.</param>
    /// <param name="maxLength">The maximum allowed length.</param>
    /// <param name="paramName">The name of the parameter (automatically captured).</param>
    /// <returns>The string value if valid.</returns>
    /// <exception cref="ArgumentException">Thrown when string exceeds maximum length.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string AgainstTooLong(string value, int maxLength, [CallerArgumentExpression(nameof(value))] string paramName = "")
    {
        AgainstNull(value, paramName);
        if (value.Length > maxLength)
            throw new ArgumentException($"Parameter '{paramName}' cannot exceed {maxLength} characters. Current length: {value.Length}.", paramName);
        return value;
    }

    /// <summary>
    /// Guards against strings that are shorter than a minimum length.
    /// </summary>
    /// <param name="value">The string value to guard.</param>
    /// <param name="minLength">The minimum required length.</param>
    /// <param name="paramName">The name of the parameter (automatically captured).</param>
    /// <returns>The string value if valid.</returns>
    /// <exception cref="ArgumentException">Thrown when string is shorter than minimum length.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string AgainstTooShort(string value, int minLength, [CallerArgumentExpression(nameof(value))] string paramName = "")
    {
        AgainstNull(value, paramName);
        if (value.Length < minLength)
            throw new ArgumentException($"Parameter '{paramName}' must be at least {minLength} characters. Current length: {value.Length}.", paramName);
        return value;
    }

    /// <summary>
    /// Guards against strings that don't match a required pattern.
    /// </summary>
    /// <param name="value">The string value to guard.</param>
    /// <param name="pattern">The regex pattern that must be matched.</param>
    /// <param name="paramName">The name of the parameter (automatically captured).</param>
    /// <returns>The string value if valid.</returns>
    /// <exception cref="ArgumentException">Thrown when string doesn't match pattern.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string AgainstInvalidFormat(string value, [StringSyntax(StringSyntaxAttribute.Regex)] string pattern, [CallerArgumentExpression(nameof(value))] string paramName = "")
    {
        AgainstNull(value, paramName);
        AgainstNullOrEmpty(pattern, nameof(pattern));
        
        if (!Regex.IsMatch(value, pattern))
            throw new ArgumentException($"Parameter '{paramName}' does not match the required format.", paramName);
        return value;
    }

    /// <summary>
    /// Guards against strings that don't match a required pattern with compiled regex.
    /// </summary>
    /// <param name="value">The string value to guard.</param>
    /// <param name="regex">The compiled regex that must be matched.</param>
    /// <param name="paramName">The name of the parameter (automatically captured).</param>
    /// <returns>The string value if valid.</returns>
    /// <exception cref="ArgumentException">Thrown when string doesn't match pattern.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string AgainstInvalidFormat(string value, Regex regex, [CallerArgumentExpression(nameof(value))] string paramName = "")
    {
        AgainstNull(value, paramName);
        AgainstNull(regex, nameof(regex));
        
        if (!regex.IsMatch(value))
            throw new ArgumentException($"Parameter '{paramName}' does not match the required format.", paramName);
        return value;
    }

    #endregion

    #region Collection Guards

    /// <summary>
    /// Guards against null or empty collections.
    /// </summary>
    /// <typeparam name="T">The type of the collection.</typeparam>
    /// <param name="collection">The collection to guard.</param>
    /// <param name="paramName">The name of the parameter (automatically captured).</param>
    /// <returns>The non-null, non-empty collection.</returns>
    /// <exception cref="ArgumentException">Thrown when collection is null or empty.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T AgainstNullOrEmpty<T>([NotNull] T? collection, [CallerArgumentExpression(nameof(collection))] string paramName = "")
        where T : ICollection
    {
        if (collection is null)
            throw new ArgumentNullException(paramName, $"Parameter '{paramName}' cannot be null.");
        if (collection.Count == 0)
            throw new ArgumentException($"Parameter '{paramName}' cannot be empty.", paramName);
        return collection;
    }

    /// <summary>
    /// Guards against collections that exceed a maximum count.
    /// </summary>
    /// <typeparam name="T">The type of the collection.</typeparam>
    /// <param name="collection">The collection to guard.</param>
    /// <param name="maxCount">The maximum allowed count.</param>
    /// <param name="paramName">The name of the parameter (automatically captured).</param>
    /// <returns>The collection if valid.</returns>
    /// <exception cref="ArgumentException">Thrown when collection exceeds maximum count.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T AgainstTooManyItems<T>(T collection, int maxCount, [CallerArgumentExpression(nameof(collection))] string paramName = "")
        where T : ICollection
    {
        AgainstNull(collection, paramName);
        if (collection.Count > maxCount)
            throw new ArgumentException($"Parameter '{paramName}' cannot contain more than {maxCount} items. Current count: {collection.Count}.", paramName);
        return collection;
    }

    /// <summary>
    /// Guards against collections that have fewer than a minimum count.
    /// </summary>
    /// <typeparam name="T">The type of the collection.</typeparam>
    /// <param name="collection">The collection to guard.</param>
    /// <param name="minCount">The minimum required count.</param>
    /// <param name="paramName">The name of the parameter (automatically captured).</param>
    /// <returns>The collection if valid.</returns>
    /// <exception cref="ArgumentException">Thrown when collection has fewer than minimum count.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T AgainstTooFewItems<T>(T collection, int minCount, [CallerArgumentExpression(nameof(collection))] string paramName = "")
        where T : ICollection
    {
        AgainstNull(collection, paramName);
        if (collection.Count < minCount)
            throw new ArgumentException($"Parameter '{paramName}' must contain at least {minCount} items. Current count: {collection.Count}.", paramName);
        return collection;
    }

    #endregion

    #region Numeric Guards

    /// <summary>
    /// Guards against negative values.
    /// </summary>
    /// <typeparam name="T">The numeric type.</typeparam>
    /// <param name="value">The value to guard.</param>
    /// <param name="paramName">The name of the parameter (automatically captured).</param>
    /// <returns>The non-negative value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when value is negative.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T AgainstNegative<T>(T value, [CallerArgumentExpression(nameof(value))] string paramName = "")
        where T : IComparable<T>, INumberBase<T>
    {
        if (value.CompareTo(T.Zero) < 0)
            throw new ArgumentOutOfRangeException(paramName, value, $"Parameter '{paramName}' cannot be negative.");
        return value;
    }

    /// <summary>
    /// Guards against negative or zero values.
    /// </summary>
    /// <typeparam name="T">The numeric type.</typeparam>
    /// <param name="value">The value to guard.</param>
    /// <param name="paramName">The name of the parameter (automatically captured).</param>
    /// <returns>The positive value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when value is negative or zero.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T AgainstNegativeOrZero<T>(T value, [CallerArgumentExpression(nameof(value))] string paramName = "")
        where T : IComparable<T>, INumberBase<T>
    {
        if (value.CompareTo(T.Zero) <= 0)
            throw new ArgumentOutOfRangeException(paramName, value, $"Parameter '{paramName}' must be positive.");
        return value;
    }

    /// <summary>
    /// Guards against zero values.
    /// </summary>
    /// <typeparam name="T">The numeric type.</typeparam>
    /// <param name="value">The value to guard.</param>
    /// <param name="paramName">The name of the parameter (automatically captured).</param>
    /// <returns>The non-zero value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when value is zero.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T AgainstZero<T>(T value, [CallerArgumentExpression(nameof(value))] string paramName = "")
        where T : IComparable<T>, INumberBase<T>
    {
        if (value.CompareTo(T.Zero) == 0)
            throw new ArgumentOutOfRangeException(paramName, value, $"Parameter '{paramName}' cannot be zero.");
        return value;
    }

    /// <summary>
    /// Guards against values outside a specified range.
    /// </summary>
    /// <typeparam name="T">The comparable type.</typeparam>
    /// <param name="value">The value to guard.</param>
    /// <param name="min">The minimum allowed value (inclusive).</param>
    /// <param name="max">The maximum allowed value (inclusive).</param>
    /// <param name="paramName">The name of the parameter (automatically captured).</param>
    /// <returns>The value if within range.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when value is outside the specified range.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T AgainstOutOfRange<T>(T value, T min, T max, [CallerArgumentExpression(nameof(value))] string paramName = "")
        where T : IComparable<T>
    {
        if (value.CompareTo(min) < 0 || value.CompareTo(max) > 0)
            throw new ArgumentOutOfRangeException(paramName, value, $"Parameter '{paramName}' must be between {min} and {max} (inclusive).");
        return value;
    }

    /// <summary>
    /// Guards against values less than a minimum value.
    /// </summary>
    /// <typeparam name="T">The comparable type.</typeparam>
    /// <param name="value">The value to guard.</param>
    /// <param name="min">The minimum allowed value (inclusive).</param>
    /// <param name="paramName">The name of the parameter (automatically captured).</param>
    /// <returns>The value if valid.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when value is less than minimum.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T AgainstTooSmall<T>(T value, T min, [CallerArgumentExpression(nameof(value))] string paramName = "")
        where T : IComparable<T>
    {
        if (value.CompareTo(min) < 0)
            throw new ArgumentOutOfRangeException(paramName, value, $"Parameter '{paramName}' cannot be less than {min}.");
        return value;
    }

    /// <summary>
    /// Guards against values greater than a maximum value.
    /// </summary>
    /// <typeparam name="T">The comparable type.</typeparam>
    /// <param name="value">The value to guard.</param>
    /// <param name="max">The maximum allowed value (inclusive).</param>
    /// <param name="paramName">The name of the parameter (automatically captured).</param>
    /// <returns>The value if valid.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when value is greater than maximum.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T AgainstTooLarge<T>(T value, T max, [CallerArgumentExpression(nameof(value))] string paramName = "")
        where T : IComparable<T>
    {
        if (value.CompareTo(max) > 0)
            throw new ArgumentOutOfRangeException(paramName, value, $"Parameter '{paramName}' cannot be greater than {max}.");
        return value;
    }

    #endregion

    #region Enum Guards

    /// <summary>
    /// Guards against undefined enum values.
    /// </summary>
    /// <typeparam name="T">The enum type.</typeparam>
    /// <param name="value">The enum value to guard.</param>
    /// <param name="paramName">The name of the parameter (automatically captured).</param>
    /// <returns>The valid enum value.</returns>
    /// <exception cref="ArgumentException">Thrown when enum value is not defined.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T AgainstUndefinedEnum<T>(T value, [CallerArgumentExpression(nameof(value))] string paramName = "")
        where T : struct, Enum
    {
        if (!Enum.IsDefined<T>(value))
            throw new ArgumentException($"Parameter '{paramName}' has an undefined enum value: {value}.", paramName);
        return value;
    }

    #endregion

    #region Type Guards

    /// <summary>
    /// Guards against values that are not of the expected type.
    /// </summary>
    /// <typeparam name="T">The expected type.</typeparam>
    /// <param name="value">The value to guard.</param>
    /// <param name="paramName">The name of the parameter (automatically captured).</param>
    /// <returns>The value cast to the expected type.</returns>
    /// <exception cref="ArgumentException">Thrown when value is not of expected type.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T AgainstInvalidType<T>(object value, [CallerArgumentExpression(nameof(value))] string paramName = "")
    {
        AgainstNull(value, paramName);
        if (value is not T result)
            throw new ArgumentException($"Parameter '{paramName}' must be of type {typeof(T).Name}. Actual type: {value.GetType().Name}.", paramName);
        return result;
    }

    #endregion

    #region Date/Time Guards

    /// <summary>
    /// Guards against dates in the future.
    /// </summary>
    /// <param name="value">The date value to guard.</param>
    /// <param name="paramName">The name of the parameter (automatically captured).</param>
    /// <returns>The date if valid.</returns>
    /// <exception cref="ArgumentException">Thrown when date is in the future.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DateTime AgainstFutureDate(DateTime value, [CallerArgumentExpression(nameof(value))] string paramName = "")
    {
        if (value > DateTime.Now)
            throw new ArgumentException($"Parameter '{paramName}' cannot be a future date. Value: {value}.", paramName);
        return value;
    }

    /// <summary>
    /// Guards against dates in the past.
    /// </summary>
    /// <param name="value">The date value to guard.</param>
    /// <param name="paramName">The name of the parameter (automatically captured).</param>
    /// <returns>The date if valid.</returns>
    /// <exception cref="ArgumentException">Thrown when date is in the past.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DateTime AgainstPastDate(DateTime value, [CallerArgumentExpression(nameof(value))] string paramName = "")
    {
        if (value < DateTime.Now)
            throw new ArgumentException($"Parameter '{paramName}' cannot be a past date. Value: {value}.", paramName);
        return value;
    }

    /// <summary>
    /// Guards against dates outside a specified range.
    /// </summary>
    /// <param name="value">The date value to guard.</param>
    /// <param name="minDate">The minimum allowed date.</param>
    /// <param name="maxDate">The maximum allowed date.</param>
    /// <param name="paramName">The name of the parameter (automatically captured).</param>
    /// <returns>The date if within range.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when date is outside the specified range.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DateTime AgainstDateOutOfRange(DateTime value, DateTime minDate, DateTime maxDate, [CallerArgumentExpression(nameof(value))] string paramName = "")
    {
        if (value < minDate || value > maxDate)
            throw new ArgumentOutOfRangeException(paramName, value, $"Parameter '{paramName}' must be between {minDate:yyyy-MM-dd} and {maxDate:yyyy-MM-dd}.");
        return value;
    }

    #endregion

    #region General Conditional Guards

    /// <summary>
    /// Guards against a condition being true.
    /// </summary>
    /// <param name="condition">The condition to guard against.</param>
    /// <param name="message">The error message if condition is true.</param>
    /// <param name="paramName">The name of the parameter.</param>
    /// <exception cref="ArgumentException">Thrown when condition is true.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Against(bool condition, string message, string? paramName = null)
    {
        if (condition)
            throw new ArgumentException(message, paramName);
    }

    /// <summary>
    /// Guards against a condition being true with a custom exception factory.
    /// </summary>
    /// <param name="condition">The condition to guard against.</param>
    /// <param name="exceptionFactory">A function that creates the exception to throw.</param>
    /// <exception cref="Exception">Thrown when condition is true.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Against(bool condition, Func<Exception> exceptionFactory)
    {
        AgainstNull(exceptionFactory, nameof(exceptionFactory));
        if (condition)
            throw exceptionFactory();
    }

    /// <summary>
    /// Guards against a condition being false (ensures condition is true).
    /// </summary>
    /// <param name="condition">The condition that must be true.</param>
    /// <param name="message">The error message if condition is false.</param>
    /// <param name="paramName">The name of the parameter.</param>
    /// <exception cref="ArgumentException">Thrown when condition is false.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Ensure(bool condition, string message, string? paramName = null)
    {
        if (!condition)
            throw new ArgumentException(message, paramName);
    }

    /// <summary>
    /// Guards against a condition being false with a custom exception factory.
    /// </summary>
    /// <param name="condition">The condition that must be true.</param>
    /// <param name="exceptionFactory">A function that creates the exception to throw.</param>
    /// <exception cref="Exception">Thrown when condition is false.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Ensure(bool condition, Func<Exception> exceptionFactory)
    {
        AgainstNull(exceptionFactory, nameof(exceptionFactory));
        if (!condition)
            throw exceptionFactory();
    }

    #endregion

    #region Guid Guards

    /// <summary>
    /// Guards against empty Guid values.
    /// </summary>
    /// <param name="value">The Guid value to guard.</param>
    /// <param name="paramName">The name of the parameter (automatically captured).</param>
    /// <returns>The non-empty Guid.</returns>
    /// <exception cref="ArgumentException">Thrown when Guid is empty.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Guid AgainstEmpty(Guid value, [CallerArgumentExpression(nameof(value))] string paramName = "")
    {
        if (value == Guid.Empty)
            throw new ArgumentException($"Parameter '{paramName}' cannot be an empty Guid.", paramName);
        return value;
    }

    #endregion

    #region File System Guards

    /// <summary>
    /// Guards against invalid file paths.
    /// </summary>
    /// <param name="path">The file path to guard.</param>
    /// <param name="paramName">The name of the parameter (automatically captured).</param>
    /// <returns>The valid file path.</returns>
    /// <exception cref="ArgumentException">Thrown when path is invalid or file doesn't exist.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string AgainstInvalidFilePath(string path, [CallerArgumentExpression(nameof(path))] string paramName = "")
    {
        AgainstNullOrWhiteSpace(path, paramName);
        
        try
        {
            var fullPath = Path.GetFullPath(path);
            if (!File.Exists(fullPath))
                throw new ArgumentException($"File does not exist at path: '{path}'.", paramName);
            return path;
        }
        catch (Exception ex) when (ex is not ArgumentException)
        {
            throw new ArgumentException($"Parameter '{paramName}' contains an invalid file path: '{path}'.", paramName, ex);
        }
    }

    /// <summary>
    /// Guards against invalid directory paths.
    /// </summary>
    /// <param name="path">The directory path to guard.</param>
    /// <param name="paramName">The name of the parameter (automatically captured).</param>
    /// <returns>The valid directory path.</returns>
    /// <exception cref="ArgumentException">Thrown when path is invalid or directory doesn't exist.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string AgainstInvalidDirectoryPath(string path, [CallerArgumentExpression(nameof(path))] string paramName = "")
    {
        AgainstNullOrWhiteSpace(path, paramName);
        
        try
        {
            var fullPath = Path.GetFullPath(path);
            if (!Directory.Exists(fullPath))
                throw new ArgumentException($"Directory does not exist at path: '{path}'.", paramName);
            return path;
        }
        catch (Exception ex) when (ex is not ArgumentException)
        {
            throw new ArgumentException($"Parameter '{paramName}' contains an invalid directory path: '{path}'.", paramName, ex);
        }
    }

    #endregion
}
