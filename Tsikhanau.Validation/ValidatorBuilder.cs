using Tsikhanau.Foundation.General;
using Tsikhanau.Foundation.Validation;
using Tsikhanau.Monads.Errors;
using Tsikhanau.Monads.Result;

namespace Tsikhanau.Validation;

public class ValidatorBuilder<TObject>
{
    private readonly TObject _object;
    private readonly List<ValidationTemplate<TObject>> _templates = [];
    
    private ValidatorBuilder(TObject @object)
    {
        _object = @object;
    }

    public static ValidatorBuilder<TObject> ForObject(TObject @object)
    {
        Guard.AgainstNull(@object);
        return new ValidatorBuilder<TObject>(@object);
    }

    public ValidatorBuilder<TObject> AddTemplate(ValidationTemplate<TObject> template)
    {
        Guard.AgainstNull(template);
        _templates.Add(template);
        return this;
    }

    public Result<Unit, ValidationError> Validate()
    {
        if (_templates.Count == 0)
            return Unit.Value;

        var errors = _templates
            .Select(template => template.Validate(_object))
            .Where(result => result.IsFailure)
            .Select(result => result.Error!)
            .ToList();

        return errors.Count == 0 
            ? Unit.Value 
            : ValidationError.From(errors);
    }
}