using Tsikhanau.Foundation.General;
using Tsikhanau.Foundation.Validation;
using Tsikhanau.Outcomes.Errors;
using Tsikhanau.Outcomes.Result;
using Tsikhanau.Validation.Abstractions;

namespace Tsikhanau.Validation;

public class ValidationTemplate<TObject>
{
    private List<IFieldValidator<TObject>> _fieldValidators;
    private List<ValidationRule<TObject>> _objectRules;
    
    private ValidationTemplate(List<IFieldValidator<TObject>> fieldValidators,
        List<ValidationRule<TObject>> objectRules)
    {
        _fieldValidators = fieldValidators;
        _objectRules = objectRules;
    }

    public static ValidationTemplate<TObject> Create(
        List<IFieldValidator<TObject>> fieldValidators,
        List<ValidationRule<TObject>> objectRules)
    {
        Guard.AgainstNullOrEmpty(fieldValidators);
        Guard.AgainstNullOrEmpty(objectRules);
        
        return new ValidationTemplate<TObject>(fieldValidators, objectRules);
    }

    public Result<Unit, ValidationError> Validate(TObject @object)
    {
        List<ValidationError> errors = [
            .._fieldValidators
                .Select(x => x.Validate(@object))
                .Where(x => x.IsFailure)
                .Select(x => x.Error),
            .._objectRules
                .Select(x => x.Validate(@object))
                .Where(x => x.IsFailure)
                .Select(x => x.Error)
        ];
        
        if (errors.Any())
            return ValidationError.From(errors);

        return Unit.Value;
    }
}