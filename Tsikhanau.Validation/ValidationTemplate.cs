using Tsikhanau.Foundation.General;
using Tsikhanau.Foundation.Validation;
using Tsikhanau.Monads.Errors;
using Tsikhanau.Monads.Result;
using Tsikhanau.Packages.ValueObjects.String;
using Tsikhanau.Validation.Abstractions;
using Tsikhanau.Validation.Builders;

namespace Tsikhanau.Validation;

public class ValidationTemplate<TObject> where TObject : notnull
{
    private readonly List<IFieldValidator<TObject>> _fieldValidators;
    private readonly List<ValidationRule<TObject>> _objectRules;
    private readonly List<AsyncValidationRule<TObject>> _asyncObjectRules;

    private ValidationTemplate(List<IFieldValidator<TObject>> fieldValidators,
        List<ValidationRule<TObject>> objectRules,
        List<AsyncValidationRule<TObject>> asyncObjectRules)
    {
        _fieldValidators = fieldValidators;
        _objectRules = objectRules;
        _asyncObjectRules = asyncObjectRules;
    }

    public static ValidationTemplateBuilder<TObject> Create()
    {
        return ValidationTemplateBuilder<TObject>.New();
    }

    internal static ValidationTemplate<TObject> CreateInternal(
        List<IFieldValidator<TObject>> fieldValidators,
        List<ValidationRule<TObject>> objectRules,
        List<AsyncValidationRule<TObject>>? asyncObjectRules = null)
    {
        Guard.AgainstNull(fieldValidators);
        Guard.AgainstNull(objectRules);

        return new ValidationTemplate<TObject>(fieldValidators, objectRules, asyncObjectRules ?? []);
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

    public Result<TObject, ValidationError> ValidateAndReturn(TObject @object)
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

        return @object;
    }

    public async Task<Result<Unit, ValidationError>> ValidateAsync(TObject @object)
    {
        var fieldResults = await Task.WhenAll(
            _fieldValidators.Select(x => x.ValidateAsync(@object)));

        var asyncObjResults = await Task.WhenAll(
            _asyncObjectRules.Select(x => x.ValidateAsync(@object)));

        List<ValidationError> errors = [
            ..fieldResults
                .Where(x => x.IsFailure)
                .Select(x => x.Error),
            .._objectRules
                .Select(x => x.Validate(@object))
                .Where(x => x.IsFailure)
                .Select(x => x.Error),
            ..asyncObjResults
                .Where(x => x.IsFailure)
                .Select(x => x.Error)
        ];

        if (errors.Any())
            return ValidationError.From(errors);

        return Unit.Value;
    }

    public async Task<Result<TObject, ValidationError>> ValidateAndReturnAsync(TObject @object)
    {
        var result = await ValidateAsync(@object);
        return result.IsSuccess ? @object : result.Error;
    }
}