using Tsikhanau.Foundation.Validation;

namespace Tsikhanau.Monads.Optional;

public readonly struct Optional<T> : IEquatable<Optional<T>>
{
    private readonly T? _value;
    private readonly Boolean _hasValue;

    private Optional(T value)
    {
        _value = value;
        _hasValue = true;
    }

    public Boolean HasValue => _hasValue;

    public Boolean IsNone => !_hasValue;

    public T Value => _hasValue ? _value! : throw new InvalidOperationException("Cannot access Value on an empty optional.");

    public static Optional<T> Some(T value)
    {
        Guard.AgainstNull(value);
        return new Optional<T>(value);
    }

    public static Optional<T> None()
    {
        return default;
    }

    public static Optional<T> FromNullable(T? value)
    {
        return value is not null ? Some(value) : None();
    }

    public T GetValueOrDefault(T defaultValue = default!)
    {
        return _hasValue ? _value! : defaultValue;
    }

    public T GetValueOrDefault(Func<T> defaultValueFactory)
    {
        Guard.AgainstNull(defaultValueFactory);
        return _hasValue ? _value! : defaultValueFactory();
    }

    public T? ToNullable()
    {
        return _hasValue ? _value : default;
    }

    public Boolean Equals(Optional<T> other)
    {
        if (_hasValue != other._hasValue)
            return false;

        if (!_hasValue)
            return true;

        return EqualityComparer<T>.Default.Equals(_value, other._value);
    }

    public override Boolean Equals(Object? obj)
    {
        return obj is Optional<T> other && Equals(other);
    }

    public override Int32 GetHashCode()
    {
        if (!_hasValue)
            return 0;

        return _value?.GetHashCode() ?? 0;
    }

    public override String ToString()
    {
        return _hasValue ? $"Some({_value})" : "None";
    }

    public static Boolean operator ==(Optional<T> left, Optional<T> right)
    {
        return left.Equals(right);
    }

    public static Boolean operator !=(Optional<T> left, Optional<T> right)
    {
        return !left.Equals(right);
    }

    public static implicit operator Optional<T>(T? value)
    {
        return value is not null ? Some(value) : None();
    }

    public static implicit operator T?(Optional<T> optional)
    {
        return optional.ToNullable();
    }
}
