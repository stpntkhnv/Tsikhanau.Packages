using System.Linq.Expressions;
using Tsikhanau.Foundation.General;
using Tsikhanau.Foundation.Validation;
using Tsikhanau.Outcomes.Result;

namespace Tsikhanau.Outcomes.Validation;

public sealed class Validator<T>
{
    private readonly T _instance;
    private readonly List<Func<T, Result<Unit, Error>>> _rules;

    private Validator(T instance)
    {
        _instance = instance;
        _rules = [];
    }

    public static Validator<T> For(T instance)
    {
        Guard.AgainstNull(instance);
        return new Validator<T>(instance);
    }

    public PropertyValidator<T, TProperty> Rule<TProperty>(Expression<Func<T, TProperty>> propertySelector)
    {
        Guard.AgainstNull(propertySelector);
        return new PropertyValidator<T, TProperty>(this, propertySelector);
    }

    internal Validator<T> AddRule(Func<T, Result<Unit, Error>> rule)
    {
        _rules.Add(rule);
        return this;
    }

    public Result<Unit, Error> Validate()
    {
        foreach (var rule in _rules)
        {
            var result = rule(_instance);
            if (result.IsFailure)
            {
                return result.Error;
            }
        }

        return Unit.Value;
    }

    public Result<Unit, Error[]> ValidateAll()
    {
        var errors = _rules
            .Select(rule => rule(_instance))
            .Where(result => result.IsFailure)
            .Select(result => result.Error)
            .ToList();

        return errors.Count == 0 
            ? Unit.Value 
            : errors.ToArray();
    }
}