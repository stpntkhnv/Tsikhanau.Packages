using System.Runtime.CompilerServices;

namespace Tsikhanau.Railway;

public static partial class OptionalExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Optional<TResult> Map<T, TResult>(
        this Optional<T> optional,
        Func<T, TResult> mapper)
    {
        return optional.HasValue
            ? Optional<TResult>.Some(mapper(optional.Value))
            : Optional<TResult>.None();
    }
}
