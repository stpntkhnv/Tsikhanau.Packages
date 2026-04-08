using Tsikhanau.Foundation.General;
using Tsikhanau.Foundation.Validation;
using Tsikhanau.Monads.Errors;
using Tsikhanau.Monads.Result;

namespace Tsikhanau.Validation;

public record class ValidationRule<T>
{
    private readonly Func<T, Boolean> _validationDelegate;
    private readonly String _errorMessage;

    public String ErrorMessage => _errorMessage;

    private ValidationRule(Func<T, Boolean> validationDelegate, String errorMessage)
    {
        _validationDelegate = validationDelegate;
        _errorMessage = errorMessage;
    }

    public static ValidationRule<T> WithMessage(Func<T, Boolean> validationDelegate, String errorMessage)
    {
        Guard.AgainstNull(validationDelegate);
        Guard.AgainstNullOrWhiteSpace(errorMessage);
        return new ValidationRule<T>(validationDelegate, errorMessage);
    }

    public Result<Unit, Error> Validate(T value)
    {
        if (_validationDelegate(value))
        {
            return Unit.Value;
        }

        return ValidationError.Single(_errorMessage);
    }

    public override String ToString() => $"ValidationRule<{typeof(T).Name}>: '{_errorMessage}'";
}
