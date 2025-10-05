using Tsikhanau.Foundation.Validation;
using Tsikhanau.Packages.ValueObjects.String;

namespace Tsikhanau.Validation.Builders;

public class FieldValidatorBuilder<TObject, TField>
{
    private readonly ValidationTemplateBuilder<TObject> _parent;
    private readonly Func<TObject, TField> _accessor;
    private readonly NotEmptyString _fieldName;
    private readonly List<ValidationRule<TField>> _rules = [];

    private FieldValidatorBuilder(ValidationTemplateBuilder<TObject> parent, Func<TObject, TField> accessor, NotEmptyString fieldName)
    {
        _parent = parent;
        _accessor = accessor;
        _fieldName = fieldName;
    }

    internal static FieldValidatorBuilder<TObject, TField> New(
        ValidationTemplateBuilder<TObject> parent,
        Func<TObject, TField> accessor,
        string fieldName)
    {
        Guard.AgainstNull(parent);
        Guard.AgainstNull(accessor);
        Guard.AgainstNullOrWhiteSpace(fieldName);
        
        var notEmptyFieldName = NotEmptyString.FromString(fieldName).Value;
        return new FieldValidatorBuilder<TObject, TField>(parent, accessor, notEmptyFieldName);
    }
    
    public FieldValidatorBuilder<TObject, TField> Must(Func<TField, bool> rule, string message)
    {
        Guard.AgainstNull(rule);
        Guard.AgainstNullOrWhiteSpace(message);
        
        var msg = NotEmptyString.FromString(message).Value;
        _rules.Add(ValidationRule<TField>.WithMessage(rule, msg));
        return this;
    }

    public FieldValidatorBuilder<TObject, TField2> Field<TField2>(Func<TObject, TField2> fieldAccessor, string fieldName)
    {
        FinalizeCurrentField();
        return _parent.Field(fieldAccessor, fieldName);
    }
    
    public ValidationTemplateObjectBuilder<TObject> Object()
    {
        FinalizeCurrentField();
        return _parent.Object();
    }
    
    public ValidationTemplate<TObject> Build()
    {
        FinalizeCurrentField();
        return _parent.Build();
    }
    
    private void FinalizeCurrentField()
    {
        if (_rules.Count > 0)
        {
            var fieldValidator = FieldValidator<TObject, TField>.Create(_accessor, _fieldName, _rules);
            _parent.AddFieldValidator(fieldValidator);
        }
    }
}