using Tsikhanau.Foundation.Validation;
using Tsikhanau.Outcomes.Optional;
using Tsikhanau.Packages.ValueObjects.String;
using Tsikhanau.Validation.Abstractions;
using Tsikhanau.Validation.Builders;

namespace Tsikhanau.Validation;

public class ValidationTemplateBuilder<TObject>
{
    private Optional<TObject> _object = Optional<TObject>.None();
    private readonly List<IFieldValidator<TObject>> _fieldValidators = [];
    private readonly List<ValidationRule<TObject>> _objectRules = [];

    private ValidationTemplateBuilder() { }

    public static ValidationTemplateBuilder<TObject> New() => new();

    public ValidationTemplateBuilder<TObject> ForObject(TObject @object)
    {
        Guard.AgainstNull(@object);
        _object = @object;
        return this;
    }
    
    public ValidationTemplateBuilder<TObject> AddObjectRule(Func<TObject, Boolean> rule, String message)
    {
        Guard.AgainstNullOrWhiteSpace(message);
        Guard.AgainstNull(rule);
        
        var validationRule = ValidationRule<TObject>.WithMessage(rule, NotEmptyString.FromString(message).Value);
        _objectRules.Add(validationRule);
        return this;
    }
    
    public FieldValidatorBuilder<TObject, TField> ForField<TField>(Func<TObject, TField> fieldAccessor)
    {
        return FieldValidatorBuilder<TObject, TField>.New(this, fieldAccessor);
    }
    
    public ValidationTemplateBuilder<TObject> AddFieldValidator(IFieldValidator<TObject> fieldValidator)
    {
        Guard.AgainstNull(fieldValidator);
        _fieldValidators.Add(fieldValidator);
        return this;
    }
}