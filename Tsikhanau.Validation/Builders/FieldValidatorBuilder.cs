using Tsikhanau.Foundation.Validation;

namespace Tsikhanau.Validation.Builders;

public class FieldValidatorBuilder<TObject, TField> where TObject : notnull
{
    private readonly ValidationTemplateBuilder<TObject> _parent;
    private readonly Func<TObject, TField> _accessor;
    private readonly String _fieldName;
    private readonly List<ValidationRule<TField>> _rules = [];
    private readonly List<AsyncValidationRule<TField>> _asyncRules = [];

    private FieldValidatorBuilder(ValidationTemplateBuilder<TObject> parent, Func<TObject, TField> accessor, String fieldName)
    {
        _parent = parent;
        _accessor = accessor;
        _fieldName = fieldName;
    }

    internal static FieldValidatorBuilder<TObject, TField> New(
        ValidationTemplateBuilder<TObject> parent,
        Func<TObject, TField> accessor,
        String fieldName)
    {
        Guard.AgainstNull(parent);
        Guard.AgainstNull(accessor);
        Guard.AgainstNullOrWhiteSpace(fieldName);

        return new FieldValidatorBuilder<TObject, TField>(parent, accessor, fieldName);
    }

    public FieldValidatorBuilder<TObject, TField> Must(Func<TField, Boolean> rule, String message)
    {
        Guard.AgainstNull(rule);
        Guard.AgainstNullOrWhiteSpace(message);

        _rules.Add(ValidationRule<TField>.WithMessage(rule, message));
        return this;
    }

    public FieldValidatorBuilder<TObject, TField> MustAsync(Func<TField, Task<Boolean>> rule, String message)
    {
        Guard.AgainstNull(rule);
        Guard.AgainstNullOrWhiteSpace(message);

        _asyncRules.Add(AsyncValidationRule<TField>.WithMessage(rule, message));
        return this;
    }

    public FieldValidatorBuilder<TObject, TField2> Field<TField2>(Func<TObject, TField2> fieldAccessor, String fieldName)
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
        if (_rules.Count > 0 || _asyncRules.Count > 0)
        {
            var fieldValidator = FieldValidator<TObject, TField>.Create(
                _accessor, _fieldName, _rules, _asyncRules);
            _parent.AddFieldValidator(fieldValidator);
        }
    }
}
