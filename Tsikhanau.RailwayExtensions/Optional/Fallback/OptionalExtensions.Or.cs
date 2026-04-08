using System.Runtime.CompilerServices;
using Tsikhanau.Monads.Optional;

namespace Tsikhanau.RailwayExtensions.Optional.Fallback;

public static partial class OptionalExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Optional<T> Or<T>(
        this Optional<T> optional,
        Optional<T> fallback)
    {
        return optional.HasValue ? optional : fallback;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Optional<T> OrElse<T>(
        this Optional<T> optional,
        Func<Optional<T>> fallbackFactory)
    {
        return optional.HasValue ? optional : fallbackFactory();
    }
}
