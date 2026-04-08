using Tsikhanau.Foundation.Validation;
using Tsikhanau.Monads.Errors;
using Tsikhanau.Monads.Result;
using Tsikhanau.Validation.Abstractions;

namespace Tsikhanau.Validation;

public sealed class Validator<T> : IValidator<T> where T : notnull
{
    private readonly ValidationTemplate<T> _template;

    private Validator(ValidationTemplate<T> template)
    {
        _template = template;
    }

    public static Validator<T> Create(ValidationTemplate<T> template)
    {
        Guard.AgainstNull(template);
        return new Validator<T>(template);
    }

    public Result<T, Error> Validate(T instance)
    {
        return _template.ValidateAndReturn(instance);
    }
}
