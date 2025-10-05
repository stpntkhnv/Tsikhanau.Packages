using Tsikhanau.Foundation.Validation;
using Tsikhanau.Packages.ValueObjects.String;

namespace Tsikhanau.Validation.Builders;

public class ValidationTemplateObjectBuilder<TObject>
{
    private readonly ValidationTemplateBuilder<TObject> _parent;

    internal ValidationTemplateObjectBuilder(ValidationTemplateBuilder<TObject> parent)
    {
        _parent = parent;
    }

    public ValidationTemplateObjectBuilder<TObject> Must(Func<TObject, bool> rule, string message)
    {
        Guard.AgainstNull(rule);
        Guard.AgainstNullOrWhiteSpace(message);
        
        var validationRule = ValidationRule<TObject>.WithMessage(rule, NotEmptyString.FromString(message).Value);
        _parent.AddObjectRule(validationRule);
        return this;
    }
    
    public FieldValidatorBuilder<TObject, TField> Field<TField>(Func<TObject, TField> fieldAccessor, string fieldName)
    {
        return _parent.Field(fieldAccessor, fieldName);
    }
    
    public ValidationTemplate<TObject> Build()
    {
        return _parent.Build();
    }
}