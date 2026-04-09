using Tsikhanau.Foundation.General;
using Tsikhanau.Monads;
using Tsikhanau.Monads.Errors;
using Tsikhanau.Monads.Result;
using Tsikhanau.RailwayExtensions;

namespace Tsikhanau.Flow;

public interface IFlowValidator
{
    Result<Unit, Error> ValidateFlow(IReadOnlyList<String> stepNames);
    Result<Unit, Error> ValidateStepDependencies(IReadOnlyDictionary<String, IReadOnlyList<String>> dependencies);
}

public class FlowValidator : IFlowValidator
{
    private readonly HashSet<String> _validStepNames;
    private readonly Dictionary<String, HashSet<String>> _stepDependencies;

    public FlowValidator()
    {
        _validStepNames = new HashSet<String>();
        _stepDependencies = new Dictionary<String, HashSet<String>>();
    }

    public FlowValidator WithValidStepName(String stepName)
    {
        ArgumentNullException.ThrowIfNull(stepName);
        _validStepNames.Add(stepName);
        return this;
    }

    public FlowValidator WithValidStepNames(params String[] stepNames)
    {
        ArgumentNullException.ThrowIfNull(stepNames);
        foreach (var stepName in stepNames)
        {
            _validStepNames.Add(stepName);
        }
        return this;
    }

    public FlowValidator WithStepDependency(String stepName, String dependsOn)
    {
        ArgumentNullException.ThrowIfNull(stepName);
        ArgumentNullException.ThrowIfNull(dependsOn);
        
        if (!_stepDependencies.ContainsKey(stepName))
        {
            _stepDependencies[stepName] = new HashSet<String>();
        }
        
        _stepDependencies[stepName].Add(dependsOn);
        return this;
    }

    public FlowValidator WithStepDependencies(String stepName, params String[] dependsOn)
    {
        ArgumentNullException.ThrowIfNull(stepName);
        ArgumentNullException.ThrowIfNull(dependsOn);
        
        if (!_stepDependencies.ContainsKey(stepName))
        {
            _stepDependencies[stepName] = new HashSet<String>();
        }
        
        foreach (var dependency in dependsOn)
        {
            _stepDependencies[stepName].Add(dependency);
        }
        return this;
    }

    public Result<Unit, Error> ValidateFlow(IReadOnlyList<String> stepNames)
    {
        if (stepNames == null || stepNames.Count == 0)
        {
            return Error.Create("code", "Flow cannot be empty");
        }

        var duplicateSteps = stepNames
            .GroupBy(x => x)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicateSteps.Any())
        {
            return Error.Create("code", $"Duplicate step names found: {String.Join(", ", duplicateSteps)}");
        }

        if (_validStepNames.Any())
        {
            var invalidSteps = stepNames.Where(step => !_validStepNames.Contains(step)).ToList();
            if (invalidSteps.Any())
            {
                return Error.Create("code", $"Invalid step names found: {String.Join(", ", invalidSteps)}");
            }
        }

        var dependencies = stepNames.ToDictionary(step => step, step => 
            _stepDependencies.ContainsKey(step) 
                ? _stepDependencies[step].ToList().AsReadOnly() as IReadOnlyList<String>
                : new List<String>().AsReadOnly());

        return ValidateStepDependencies(dependencies);
    }

    public Result<Unit, Error> ValidateStepDependencies(IReadOnlyDictionary<String, IReadOnlyList<String>> dependencies)
    {
        ArgumentNullException.ThrowIfNull(dependencies);

        var visited = new HashSet<String>();
        var recursionStack = new HashSet<String>();
        var stepNames = dependencies.Keys.ToList();

        foreach (var stepName in stepNames)
        {
            if (!visited.Contains(stepName))
            {
                var circularResult = DetectCircularDependency(stepName, dependencies, visited, recursionStack);
                if (circularResult.IsFailure)
                {
                    return circularResult;
                }
            }
        }

        foreach (var kvp in dependencies)
        {
            var stepName = kvp.Key;
            var stepDependencies = kvp.Value;

            foreach (var dependency in stepDependencies)
            {
                if (!stepNames.Contains(dependency))
                {
                    return Error.Create("code", $"Step '{stepName}' depends on '{dependency}' which is not present in the flow");
                }
            }
        }

        return Unit.Value;
    }

    private static Result<Unit, Error> DetectCircularDependency(
        String stepName,
        IReadOnlyDictionary<String, IReadOnlyList<String>> dependencies,
        HashSet<String> visited,
        HashSet<String> recursionStack)
    {
        visited.Add(stepName);
        recursionStack.Add(stepName);

        if (dependencies.TryGetValue(stepName, out var stepDependencies))
        {
            foreach (var dependency in stepDependencies)
            {
                if (!visited.Contains(dependency))
                {
                    var result = DetectCircularDependency(dependency, dependencies, visited, recursionStack);
                    if (result.IsFailure)
                    {
                        return result;
                    }
                }
                else if (recursionStack.Contains(dependency))
                {
                    return Error.Create("code", $"Circular dependency detected: {stepName} -> {dependency}");
                }
            }
        }

        recursionStack.Remove(stepName);
        return Unit.Value;
    }
}

public static class FlowValidatorExtensions
{
    public static Result<IFlow<TInput, TOutput>, Error> ValidateAndBuild<TInput, TOutput>(
        this FlowBuilder<TInput, TOutput> builder,
        IFlowValidator validator) where TInput : notnull where TOutput : notnull
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(validator);

        var flow = builder.Build();
        var validationResult = validator.ValidateFlow(flow.StepNames);

        return validationResult.IsSuccess 
            ? Result.Success(flow)  
            : validationResult.Error;
    }

    public static Result<IFlow<TOutput>, Error> ValidateAndBuild<TOutput>(
        this FlowBuilder<TOutput> builder,
        IFlowValidator validator) where TOutput : notnull
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(validator);

        var flow = builder.Build();
        var validationResult = validator.ValidateFlow(flow.StepNames);

        return validationResult.IsSuccess 
            ? Result.Success(flow) 
            : validationResult.Error;
    }

    public static Result<IFlow, Error> ValidateAndBuild(
        this FlowBuilder builder,
        IFlowValidator validator)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(validator);

        var flow = builder.Build();
        var validationResult = validator.ValidateFlow(flow.StepNames);

        return validationResult.IsSuccess 
            ? Result.Success(flow)  
            : validationResult.Error;
    }
}

