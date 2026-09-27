using System.Runtime.CompilerServices;
using Tsikhanau.Monads.Optional;

namespace Tsikhanau.RailwayExtensions.Optional.Match;

public static partial class OptionalExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TResult Match<T, TResult>(
        this Optional<T> optional,
        Func<T, TResult> onSome,
        Func<TResult> onNone)
    {
        return optional.HasValue
            ? onSome(optional.Value)
            : onNone();
    }
}
