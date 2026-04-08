using Tsikhanau.Foundation.General;
using Tsikhanau.Foundation.Validation;
using Tsikhanau.Monads.Errors;
using Tsikhanau.Monads.Result;
using Tsikhanau.Packages.ValueObjects.String;
using Tsikhanau.Validation.Abstractions;
using Tsikhanau.Validation.Builders;

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

    public static ValidationTemplateBuilder<TObject> Create()
    {
        return ValidationTemplateBuilder<TObject>.New();
    }

    internal static ValidationTemplate<TObject> CreateInternal(
        List<IFieldValidator<TObject>> fieldValidators,
        List<ValidationRule<TObject>> objectRules)
    {
        Guard.AgainstNull(fieldValidators);
        Guard.AgainstNull(objectRules);
        
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