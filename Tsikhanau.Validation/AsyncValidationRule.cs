using Tsikhanau.Foundation.General;
using Tsikhanau.Foundation.Validation;
using Tsikhanau.Monads.Errors;
using Tsikhanau.Monads.Result;
using Tsikhanau.Packages.ValueObjects.String;

namespace Tsikhanau.Validation;

public record class AsyncValidationRule<T>
{
    private readonly Func<T, Task<Boolean>> _validationDelegate;
    private readonly NotEmptyString _errorMessage;

    public NotEmptyString ErrorMessage => _errorMessage;

    private AsyncValidationRule(Func<T, Task<Boolean>> validationDelegate, NotEmptyString errorMessage)
    {
        _validationDelegate = validationDelegate;
        _errorMessage = errorMessage;
    }

    public static AsyncValidationRule<T> WithMessage(Func<T, Task<Boolean>> validationDelegate, NotEmptyString errorMessage)
    {
        Guard.AgainstNull(validationDelegate);
        return new AsyncValidationRule<T>(validationDelegate, errorMessage);
    }

    public async Task<Result<Unit, Error>> ValidateAsync(T value)
    {
        if (await _validationDelegate(value))
        {
            return Unit.Value;
        }

        return ValidationError.Single(_errorMessage.Value);
    }

    public override String ToString() => $"AsyncValidationRule<{typeof(T).Name}>: '{_errorMessage.Value}'";
}
