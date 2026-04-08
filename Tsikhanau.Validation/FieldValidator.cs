using Tsikhanau.Foundation.General;
using Tsikhanau.Foundation.Validation;
using Tsikhanau.Monads.Errors;
using Tsikhanau.Monads.Result;
using Tsikhanau.Packages.ValueObjects.String;
using Tsikhanau.Validation.Abstractions;

namespace Tsikhanau.Validation;

public class FieldValidator<TObject, TField> : IFieldValidator<TObject> where TObject : notnull
{
    private readonly Func<TObject, TField> _fieldAccessor;
    private readonly NotEmptyString _fieldName;
    private readonly List<ValidationRule<TField>> _validationRules;
    private readonly List<AsyncValidationRule<TField>> _asyncValidationRules;

    private FieldValidator(
        Func<TObject, TField> fieldAccessor,
        NotEmptyString fieldName,
        List<ValidationRule<TField>> validationRules,
        List<AsyncValidationRule<TField>> asyncValidationRules)
    {
        _fieldAccessor = fieldAccessor;
        _fieldName = fieldName;
        _validationRules = validationRules;
        _asyncValidationRules = asyncValidationRules;
    }

    public static FieldValidator<TObject, TField> Create(
        Func<TObject, TField> fieldAccessor,
        NotEmptyString fieldName,
        List<ValidationRule<TField>> validationRules,
        List<AsyncValidationRule<TField>>? asyncValidationRules = null)
    {
        Guard.AgainstNull(fieldAccessor);
        Guard.AgainstNull(validationRules);

        var asyncRules = asyncValidationRules ?? [];
        Guard.Against(validationRules.Count == 0 && asyncRules.Count == 0,
            "Collection of validation rules should not be empty.");

        return new FieldValidator<TObject, TField>(fieldAccessor, fieldName, validationRules, asyncRules);
    }

    public Result<Unit, Error> Validate(TObject obj)
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

    public async Task<Result<Unit, Error>> ValidateAsync(TObject obj)
    {
        var value = _fieldAccessor(obj);

        var syncErrors = _validationRules
            .Select(r => r.Validate(value))
            .Where(r => r.IsFailure)
            .Select(r => $"[{_fieldName.Value}] {r.Error!.Message}")
            .ToList();

        var asyncResults = await Task.WhenAll(
            _asyncValidationRules.Select(r => r.ValidateAsync(value)));

        var asyncErrors = asyncResults
            .Where(r => r.IsFailure)
            .Select(r => $"[{_fieldName.Value}] {r.Error!.Message}")
            .ToList();

        var allErrors = syncErrors.Concat(asyncErrors).ToList();

        return allErrors.Count == 0
            ? Unit.Value
            : ValidationError.From(allErrors);
    }
}