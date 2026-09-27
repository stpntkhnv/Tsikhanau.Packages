using System.Runtime.CompilerServices;
using Tsikhanau.Monads.Optional;

namespace Tsikhanau.RailwayExtensions.Optional.Filter;

public static partial class OptionalExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Optional<T> Where<T>(
        this Optional<T> optional,
        Func<T, Boolean> predicate)
    {
        return optional.HasValue && predicate(optional.Value)
            ? optional
            : Optional<T>.None();
    }
}
