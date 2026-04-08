using Tsikhanau.Foundation.Validation;

namespace Tsikhanau.Validation.Builders;

public class ValidationTemplateObjectBuilder<TObject> where TObject : notnull
{
    private readonly ValidationTemplateBuilder<TObject> _parent;

    internal ValidationTemplateObjectBuilder(ValidationTemplateBuilder<TObject> parent)
    {
        _parent = parent;
    }

    public ValidationTemplateObjectBuilder<TObject> Must(Func<TObject, Boolean> rule, String message)
    {
        Guard.AgainstNull(rule);
        Guard.AgainstNullOrWhiteSpace(message);

        _parent.AddObjectRule(ValidationRule<TObject>.WithMessage(rule, message));
        return this;
    }

    public ValidationTemplateObjectBuilder<TObject> MustAsync(Func<TObject, Task<Boolean>> rule, String message)
    {
        Guard.AgainstNull(rule);
        Guard.AgainstNullOrWhiteSpace(message);

        _parent.AddAsyncObjectRule(AsyncValidationRule<TObject>.WithMessage(rule, message));
        return this;
    }

    public FieldValidatorBuilder<TObject, TField> Field<TField>(Func<TObject, TField> fieldAccessor, String fieldName)
    {
        return _parent.Field(fieldAccessor, fieldName);
    }

    public ValidationTemplate<TObject> Build()
    {
        return _parent.Build();
    }
}
