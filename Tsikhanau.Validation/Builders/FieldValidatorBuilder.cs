using Tsikhanau.Foundation.Validation;
using Tsikhanau.Packages.ValueObjects.String;

namespace Tsikhanau.Validation.Builders;

public class FieldValidatorBuilder<TObject, TField>
{
    private readonly ValidationTemplateBuilder<TObject> _parent;
    private readonly Func<TObject, TField> _accessor;
    private NotEmptyString _fieldName;
    private readonly List<ValidationRule<TField>> _rules = [];

    private FieldValidatorBuilder(ValidationTemplateBuilder<TObject> parent, Func<TObject, TField> accessor)
    {
        _parent = parent;
        _accessor = accessor;
    }

    public static FieldValidatorBuilder<TObject, TField> New(
        ValidationTemplateBuilder<TObject> parent,
        Func<TObject, TField> accessor)
    {
        Guard.AgainstNull(parent);
        Guard.AgainstNull(accessor);
        return new FieldValidatorBuilder<TObject, TField>(parent, accessor);
    }

    public FieldValidatorBuilder<TObject, TField> WithFieldName(NotEmptyString fieldName)
    {
        _fieldName = fieldName;
        return this;
    }
    
    public FieldValidatorBuilder<TObject, TField> WithFieldName(string fieldName)
    {
        Guard.AgainstNullOrWhiteSpace(fieldName);
        _fieldName = NotEmptyString.FromString(fieldName).Value;
        return this;
    }
    
    public FieldValidatorBuilder<TObject, TField> AddRule(Func<TField, Boolean> rule, String message)
    {
        Guard.AgainstNull(rule);
        Guard.AgainstNullOrEmpty(message);
        
        var msg = NotEmptyString.FromString(message).Value;
        _rules.Add(ValidationRule<TField>.WithMessage(rule, msg));
        return this;
    }

    public ValidationTemplateBuilder<TObject> Build()
    {
        Guard.AgainstNull(_accessor, nameof(_accessor));
        Guard.AgainstNull(_fieldName, nameof(_fieldName));
        Guard.AgainstNullOrEmpty(_rules, nameof(_rules));
        
        var fieldValidator = FieldValidator<TObject, TField>.Create(_accessor, _fieldName, _rules);
        _parent.AddFieldValidator(fieldValidator);

        return _parent;
    }
}