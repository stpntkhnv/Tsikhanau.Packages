using System.Runtime.CompilerServices;

namespace Tsikhanau.Foundation.Validation;

public static class Ensure
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void That(Boolean condition, String message)
    {
        if (!condition)
            throw new InvalidOperationException(message);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void IsTrue(Boolean condition, String message)
    {
        That(condition, message);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void IsFalse(Boolean condition, String message)
    {
        if (condition)
            throw new InvalidOperationException(message);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void IsNotNull<T>(T? value, String message)
    {
        if (value is not null)
            throw new InvalidOperationException(message);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void IsNull<T>(T? value, String message)
    {
        if (value is null)
            throw new InvalidOperationException(message);
    }
}