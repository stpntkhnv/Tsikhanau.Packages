using System.Linq.Expressions;
using Tsikhanau.Foundation.General;
using Tsikhanau.Outcomes.Result;

namespace Tsikhanau.Outcomes.Validation;

public sealed class PropertyValidator<T, TProperty>
{
    private readonly Validator<T> _validator;
    private readonly Func<T, TProperty> _propertyAccessor;
    private readonly String _propertyName;

    internal PropertyValidator(Validator<T> validator, Expression<Func<T, TProperty>> propertySelector)
    {
        _validator = validator;
        _propertyAccessor = propertySelector.Compile();
        _propertyName = GetPropertyName(propertySelector);
    }

    private static String GetPropertyName(Expression<Func<T, TProperty>> propertySelector)
    {
        return propertySelector.Body switch
        {
            MemberExpression memberExpression => memberExpression.Member.Name,
            _ => throw new ArgumentException("Property selector must be a member expression", nameof(propertySelector))
        };
    }

    public PropertyValidator<T, TProperty> Must(Func<TProperty, Boolean> predicate, String? errorMessage = null)
    {
        var rule = CreateRule(predicate, errorMessage ?? $"{_propertyName} does not meet the specified condition");
        return AddRule(rule);
    }

    public PropertyValidator<T, TProperty> Must(Func<TProperty, Boolean> predicate, Error error)
    {
        var rule = CreateRule(predicate, error);
        return AddRule(rule);
    }

    public PropertyValidator<T, TOtherProperty> Rule<TOtherProperty>(Expression<Func<T, TOtherProperty>> propertySelector)
    {
        return _validator.Rule(propertySelector);
    }

    public Result<Unit, Error> Validate() => _validator.Validate();

    public Result<Unit, Error[]> ValidateAll() => _validator.ValidateAll();

    private Func<T, Result<Unit, Error>> CreateRule(Func<TProperty, Boolean> predicate, String errorMessage)
    {
        return instance =>
        {
            var value = _propertyAccessor(instance);
            return predicate(value) 
                ? Unit.Value 
                : Error.Validation(errorMessage);
        };
    }

    private Func<T, Result<Unit, Error>> CreateRule(Func<TProperty, Boolean> predicate, Error error)
    {
        return instance =>
        {
            var value = _propertyAccessor(instance);
            return predicate(value) 
                ? Unit.Value 
                : error;
        };
    }

    private PropertyValidator<T, TProperty> AddRule(Func<T, Result<Unit, Error>> rule)
    {
        _validator.AddRule(rule);
        return this;
    }
}