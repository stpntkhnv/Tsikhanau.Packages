using System.Runtime.CompilerServices;
using Tsikhanau.Monads.Optional;

namespace Tsikhanau.RailwayExtensions.Optional.SideEffects;

public static partial class OptionalExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Optional<T> Tap<T>(
        this Optional<T> optional,
        Action<T> action)
    {
        if (optional.HasValue)
        {
            action(optional.Value);
        }

        return optional;
    }
}
