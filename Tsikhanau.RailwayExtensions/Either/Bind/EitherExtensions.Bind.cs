using System.Runtime.CompilerServices;
using Tsikhanau.Monads.Either;

namespace Tsikhanau.RailwayExtensions.Either.Bind;

public static partial class EitherExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Either<TLeft, TResult> Bind<TLeft, TRight, TResult>(
        this Either<TLeft, TRight> either,
        Func<TRight, Either<TLeft, TResult>> binder)
    {
        return either.IsRight
            ? binder(either.Right)
            : Either<TLeft, TResult>.FromLeft(either.Left);
    }
}
