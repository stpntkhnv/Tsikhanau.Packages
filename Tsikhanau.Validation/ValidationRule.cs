using Tsikhanau.Foundation.General;
using Tsikhanau.Foundation.Validation;
using Tsikhanau.Outcomes.Errors;
using Tsikhanau.Outcomes.Result;
using Tsikhanau.Packages.ValueObjects.String;

namespace Tsikhanau.Validation;

public record class ValidationRule<T>
{
    private readonly Func<T, Boolean> _validationDelegate;
    private readonly NotEmptyString _errorMessage;
    
    public NotEmptyString ErrorMessage => _errorMessage;

    private ValidationRule(Func<T, Boolean> validationDelegate, NotEmptyString errorMessage)
    {
        _validationDelegate = validationDelegate;
        _errorMessage = errorMessage;
    }

    public static ValidationRule<T> WithMessage(Func<T, Boolean> validationDelegate, NotEmptyString errorMessage)
    {
        Guard.AgainstNull(validationDelegate);
        return new ValidationRule<T>(validationDelegate, errorMessage);
    }

    public Result<Unit, ValidationError> Validate(T value)
    {
        if (_validationDelegate(value))
        {
            return Unit.Value;
        };

        return ValidationError.Single(_errorMessage.Value);
    }
    
    public override String ToString() => $"ValidationRule<{typeof(T).Name}>: '{_errorMessage.Value}'";
}