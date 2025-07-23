using Tsikhanau.Foundation.General;
using Tsikhanau.Foundation.Validation;
using Tsikhanau.Outcomes.Errors;
using Tsikhanau.Outcomes.Result;
using Tsikhanau.Packages.ValueObjects.String;
using Tsikhanau.Validation.Abstractions;

namespace Tsikhanau.Validation;

public class FieldValidator<TObject, TField> : IFieldValidator<TObject>
{
    private readonly Func<TObject, TField> _fieldAccessor;
    private readonly NotEmptyString _fieldName;
    private readonly List<ValidationRule<TField>> _validationRules;

    private FieldValidator(
        Func<TObject, TField> fieldAccessor,
        NotEmptyString fieldName,
        List<ValidationRule<TField>> validationRules)
    {
        _fieldAccessor = fieldAccessor;
        _fieldName = fieldName;
        _validationRules = validationRules;
    }

    public static FieldValidator<TObject, TField> Create(
        Func<TObject, TField> fieldAccessor,
        NotEmptyString fieldName,
        List<ValidationRule<TField>> validationRules)
    {
        Guard.AgainstNull(fieldAccessor);
        Guard.AgainstNull(validationRules);
        Guard.Against(validationRules.Count == 0, "Collection of validation rules should not be empty.");
        
        return new FieldValidator<TObject, TField>(fieldAccessor, fieldName, validationRules);
    }

    public Result<Unit, ValidationError> Validate(TObject obj)
    {
        var value = _fieldAccessor(obj);
        var errors = _validationRules
            .Select(r => r.Validate(value))
            .Where(r => r.IsFailure)
            .Select(r => $"[{_fieldName.Value}] {r.Error!.Message}")
            .ToList();

        return errors.Count == 0 
            ? Unit.Value 
            : ValidationError.From(errors);
    }
}