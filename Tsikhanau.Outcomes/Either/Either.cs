using Tsikhanau.Foundation.Validation;

namespace Tsikhanau.Outcomes.Either;

public readonly struct Either<TLeft, TRight> : IEquatable<Either<TLeft, TRight>>
{
    private readonly TLeft? _left;
    private readonly TRight? _right;
    private readonly Boolean _isRight;

    private Either(TLeft left)
    {
        _left = left;
        _right = default;
        _isRight = false;
    }

    private Either(TRight right)
    {
        _left = default;
        _right = right;
        _isRight = true;
    }

    public Boolean IsLeft => !_isRight;

    public Boolean IsRight => _isRight;

    public TLeft Left => !_isRight ? _left! : throw new InvalidOperationException($"Cannot access Left value on a Right Either. Right value: {_right}");

    public TRight Right => _isRight ? _right! : throw new InvalidOperationException($"Cannot access Right value on a Left Either. Left value: {_left}");

    public static Either<TLeft, TRight> FromLeft(TLeft left)
    {
        Guard.AgainstNull(left);
        return new Either<TLeft, TRight>(left);
    }

    public static Either<TLeft, TRight> FromRight(TRight right)
    {
        Guard.AgainstNull(right);
        return new Either<TLeft, TRight>(right);
    }

    public TLeft GetLeftOrDefault(TLeft defaultValue = default!)
    {
        return !_isRight ? _left! : defaultValue;
    }

    public TRight GetRightOrDefault(TRight defaultValue = default!)
    {
        return _isRight ? _right! : defaultValue;
    }

    public TResult Match<TResult>(Func<TLeft, TResult> leftFunc, Func<TRight, TResult> rightFunc)
    {
        Guard.AgainstNull(leftFunc);
        Guard.AgainstNull(rightFunc);

        return _isRight ? rightFunc(_right!) : leftFunc(_left!);
    }

    public void Match(Action<TLeft> leftAction, Action<TRight> rightAction)
    {
        Guard.AgainstNull(leftAction);
        Guard.AgainstNull(rightAction);

        if (_isRight)
            rightAction(_right!);
        else
            leftAction(_left!);
    }

    public Boolean Equals(Either<TLeft, TRight> other)
    {
        if (_isRight != other._isRight)
            return false;

        if (_isRight)
            return EqualityComparer<TRight>.Default.Equals(_right, other._right);

        return EqualityComparer<TLeft>.Default.Equals(_left, other._left);
    }

    public override Boolean Equals(Object? obj)
    {
        return obj is Either<TLeft, TRight> other && Equals(other);
    }

    public override Int32 GetHashCode()
    {
        if (_isRight)
            return HashCode.Combine(_isRight, _right);

        return HashCode.Combine(_isRight, _left);
    }

    public override String ToString()
    {
        return _isRight ? $"Right({_right})" : $"Left({_left})";
    }

    public static Boolean operator ==(Either<TLeft, TRight> left, Either<TLeft, TRight> right)
    {
        return left.Equals(right);
    }

    public static Boolean operator !=(Either<TLeft, TRight> left, Either<TLeft, TRight> right)
    {
        return !left.Equals(right);
    }

    public static implicit operator Either<TLeft, TRight>(TLeft left)
    {
        return FromLeft(left);
    }

    public static implicit operator Either<TLeft, TRight>(TRight right)
    {
        return FromRight(right);
    }
}
