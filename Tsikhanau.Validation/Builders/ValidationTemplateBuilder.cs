using Tsikhanau.Foundation.Validation;
using Tsikhanau.Validation.Abstractions;
using Tsikhanau.Validation.Builders;

namespace Tsikhanau.Validation;

public class ValidationTemplateBuilder<TObject> where TObject : notnull
{
    private readonly List<IFieldValidator<TObject>> _fieldValidators = [];
    private readonly List<ValidationRule<TObject>> _objectRules = [];
    private readonly List<AsyncValidationRule<TObject>> _asyncObjectRules = [];
    private ValidationTemplateObjectBuilder<TObject>? _objectBuilder;

    private ValidationTemplateBuilder() { }

    internal static ValidationTemplateBuilder<TObject> New() => new();

    public FieldValidatorBuilder<TObject, TField> Field<TField>(Func<TObject, TField> fieldAccessor, string fieldName)
    {
        Guard.AgainstNull(fieldAccessor);
        Guard.AgainstNullOrWhiteSpace(fieldName);
        
        return FieldValidatorBuilder<TObject, TField>.New(this, fieldAccessor, fieldName);
    }
    
    public ValidationTemplateObjectBuilder<TObject> Object()
    {
        _objectBuilder ??= new ValidationTemplateObjectBuilder<TObject>(this);
        return _objectBuilder;
    }
    
    internal ValidationTemplateBuilder<TObject> AddFieldValidator(IFieldValidator<TObject> fieldValidator)
    {
        Guard.AgainstNull(fieldValidator);
        _fieldValidators.Add(fieldValidator);
        return this;
    }
    
    internal ValidationTemplateBuilder<TObject> AddObjectRule(ValidationRule<TObject> rule)
    {
        Guard.AgainstNull(rule);
        _objectRules.Add(rule);
        return this;
    }

    internal ValidationTemplateBuilder<TObject> AddAsyncObjectRule(AsyncValidationRule<TObject> rule)
    {
        Guard.AgainstNull(rule);
        _asyncObjectRules.Add(rule);
        return this;
    }

    public ValidationTemplate<TObject> Build()
    {
        return ValidationTemplate<TObject>.CreateInternal(_fieldValidators, _objectRules, _asyncObjectRules);
    }
}