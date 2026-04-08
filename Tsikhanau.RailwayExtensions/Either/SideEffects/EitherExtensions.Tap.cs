using System.Runtime.CompilerServices;
using Tsikhanau.Monads.Either;

namespace Tsikhanau.RailwayExtensions.Either.SideEffects;

public static partial class EitherExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Either<TLeft, TRight> Tap<TLeft, TRight>(
        this Either<TLeft, TRight> either,
        Action<TRight> action)
    {
        if (either.IsRight)
        {
            action(either.Right);
        }

        return either;
    }
}
