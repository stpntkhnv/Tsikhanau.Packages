using System.Runtime.CompilerServices;
using Tsikhanau.Monads.Either;

namespace Tsikhanau.RailwayExtensions.Either.SideEffects;

public static partial class EitherExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Either<TLeft, TRight> TapLeft<TLeft, TRight>(
        this Either<TLeft, TRight> either,
        Action<TLeft> action)
    {
        if (either.IsLeft)
        {
            action(either.Left);
        }

        return either;
    }
}
