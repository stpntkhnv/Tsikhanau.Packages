using System.Runtime.CompilerServices;
using Tsikhanau.Monads.Optional;

namespace Tsikhanau.RailwayExtensions.Optional.Bind;

public static partial class OptionalExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Optional<TResult> Bind<T, TResult>(
        this Optional<T> optional,
        Func<T, Optional<TResult>> binder)
    {
        return optional.HasValue
            ? binder(optional.Value)
            : Optional<TResult>.None();
    }
}
