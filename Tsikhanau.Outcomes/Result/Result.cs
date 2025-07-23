using System.Diagnostics;
using Tsikhanau.Foundation.Validation;

namespace Tsikhanau.Outcomes.Result;

public readonly struct Result<TData, TError> : IEquatable<Result<TData, TError>>
    where TData : notnull
    where TError : notnull
{
    private readonly TData? _data;
    private readonly TError? _error;

    private Result(TData data)
    {
        _data = data;
        _error = default;
        IsSuccess = true;
    }

    private Result(TError error)
    {
        _data = default;
        _error = error;
        IsSuccess = false;
    }

    public Boolean IsSuccess { get; }

    public Boolean IsFailure => !IsSuccess;

    public TData Value => IsSuccess 
        ? _data! 
        : throw new InvalidOperationException($"Cannot access Value on a failed result. Error: {_error}");

    public TError Error => !IsSuccess 
        ? _error! 
        : throw new InvalidOperationException("Cannot access Error on a successful result.");

    public static Result<TData, TError> Success(TData value)
    {
        Guard.AgainstNull(value);
        return new Result<TData, TError>(value);
    }

    public static Result<TData, TError> Failure(TError error)
    {
        Guard.AgainstNull(error);
        return new Result<TData, TError>(error);
    }

    public Boolean Equals(Result<TData, TError> other)
    {
        if (IsSuccess != other.IsSuccess)
        {
            return false;
        }
        
        if (IsSuccess)
        {
            return EqualityComparer<TData>.Default.Equals(_data, other._data);
        }

        return EqualityComparer<TError>.Default.Equals(_error, other._error);
    }

    public override Boolean Equals(Object? obj) => obj is Result<TData, TError> other && Equals(other);

    public override Int32 GetHashCode() 
        => IsSuccess 
            ? HashCode.Combine(IsSuccess, _data) 
            : HashCode.Combine(IsSuccess, _error);

    public override String ToString() => IsSuccess 
        ? $"Success({_data})" 
        : $"Failure({_error})";
    
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    private String DebuggerDisplay => ToString();

    public static Boolean operator ==(Result<TData, TError> left, Result<TData, TError> right) => left.Equals(right);

    public static Boolean operator !=(Result<TData, TError> left, Result<TData, TError> right) => !left.Equals(right);

    public static implicit operator Result<TData, TError>(TData value) => Success(value);

    public static implicit operator Result<TData, TError>(TError error) => Failure(error);
    
    public static implicit operator Boolean(Result<TData, TError> r) => r.IsSuccess;
}