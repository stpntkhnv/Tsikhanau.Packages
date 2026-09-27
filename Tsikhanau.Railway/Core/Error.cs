using Tsikhanau.Foundation.Validation;

namespace Tsikhanau.Monads.Errors;

public class Error : IEquatable<Error>
{
    protected Error(String code, String message, Error? innerError = null)
    {
        Code = code;
        Message = message;
        InnerError = innerError;
    }

    public String Code { get; }

    public String Message { get; }

    public Error? InnerError { get; }

    public static Error Create(String code, String message)
    {
        Guard.AgainstNullOrWhiteSpace(code);
        Guard.AgainstNullOrWhiteSpace(message);
        return new Error(code, message);
    }

    public static Error Create(String code, String message, Error innerError)
    {
        Guard.AgainstNullOrWhiteSpace(code);
        Guard.AgainstNullOrWhiteSpace(message);
        Guard.AgainstNull(innerError);
        return new Error(code, message, innerError);
    }

    public static Error FromException(Exception exception)
    {
        Guard.AgainstNull(exception);
        
        var innerError = exception.InnerException != null 
            ? FromException(exception.InnerException) 
            : null;

        return new Error(
            exception.GetType().Name,
            exception.Message,
            innerError);
    }

    public Error WithContext(String code, String message)
    {
        Guard.AgainstNullOrWhiteSpace(code);
        Guard.AgainstNullOrWhiteSpace(message);
        return new Error(code, message, this);
    }
    
    public static Error Validation(String message)
    {
        Guard.AgainstNullOrWhiteSpace(message);
        return new Error("VALIDATION", message);
    }
    
    public static readonly Error Unknown =
        new Error("UNKNOWN", "Unexpected error");

    public String GetFullMessage() => InnerError is null 
        ? Message 
        : $"{Message} -> {InnerError.GetFullMessage()}";

    public IEnumerable<String> GetAllCodes()
    {
        yield return Code;
        
        if (InnerError != null)
        {
            foreach (var code in InnerError.GetAllCodes())
            {
                yield return code;
            }
        }
    }

    public Boolean Equals(Error? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return String.Equals(Code, other.Code, StringComparison.Ordinal) &&
               String.Equals(Message, other.Message, StringComparison.Ordinal) &&
               Equals(InnerError, other.InnerError);
    }

    public override Boolean Equals(Object? obj) => obj is Error other && Equals(other);

    public override Int32 GetHashCode() => HashCode.Combine(Code, Message, InnerError);

    public override String ToString() =>
        InnerError == null 
            ? $"[{Code}] {Message}"
            : $"[{Code}] {Message} -> {InnerError}";

    public static Boolean operator ==(Error? left, Error? right) => Equals(left, right);

    public static Boolean operator !=(Error? left, Error? right) => !Equals(left, right);

    public static implicit operator Error(String message) => Create("GENERAL", message);
}