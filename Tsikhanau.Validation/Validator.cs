using Tsikhanau.Foundation.General;
using Tsikhanau.Foundation.Validation;
using Tsikhanau.Monads.Errors;
using Tsikhanau.Monads.Result;

namespace Tsikhanau.Validation;

public sealed class Validator<T>
{
    private readonly T _data;
    private readonly ValidationTemplate<T> _template;

    private Validator(T data, ValidationTemplate<T> template)
    {
        _data = data;
        _template = template;
    }

    public static Validator<T> Create(T data, ValidationTemplate<T> template)
    {
        Guard.AgainstNull(data);
        Guard.AgainstNull(template);
        return new Validator<T>(data, template);
    }

    public Result<Unit, ValidationError> Validate()
    {
        return _template.Validate(_data);
    }
}
