using Tsikhanau.Foundation.General;
using Tsikhanau.Monads;
using Tsikhanau.Monads.Errors;
using Tsikhanau.Monads.Result;
using Tsikhanau.RailwayExtensions;
using Tsikhanau.RailwayExtensions.Result;
using Tsikhanau.RailwayExtensions.Result.Map;

#pragma warning disable CS8714, CS8621

namespace Tsikhanau.Flow;

public class FlowBuilder<TInput, TOutput> where TInput : notnull where TOutput : notnull
{
    private readonly String _name;
    private readonly List<Func<Object?, FlowContext, CancellationToken, Task<Result<Object?, Error>>>> _steps = [];
    private readonly List<String> _stepNames = [];

    internal FlowBuilder(String name)
    {
        _name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public FlowBuilder<TInput, TNewOutput> Step<TNewOutput>(String stepName, Func<TOutput, FlowContext, CancellationToken, Task<Result<TNewOutput, Error>>> stepFunc) where TNewOutput : notnull
    {
        ArgumentNullException.ThrowIfNull(stepName);
        ArgumentNullException.ThrowIfNull(stepFunc);

        var newBuilder = new FlowBuilder<TInput, TNewOutput>(_name);

        foreach (var step in _steps)
        {
            newBuilder._steps.Add(step);
        }

        foreach (var name in _stepNames)
        {
            newBuilder._stepNames.Add(name);
        }

        newBuilder._steps.Add(async (input, context, ct) =>
        {
            if (input is TOutput typedInput)
            {
                var result = await stepFunc(typedInput, context, ct);
                return result.Map(r => (Object?)r);
            }
            return Error.Create("code", $"Invalid input type for step '{stepName}'. Expected {typeof(TOutput).Name}, got {input?.GetType().Name ?? "null"}");
        });

        newBuilder._stepNames.Add(stepName);

        return newBuilder;
    }

    public FlowBuilder<TInput, TNewOutput> Step<TNewOutput>(String stepName, ITransformStep<TOutput, TNewOutput> step) where TNewOutput : notnull
    {
        ArgumentNullException.ThrowIfNull(step);
        return Step(stepName, step.ExecuteAsync);
    }

    public FlowBuilder<TInput, TOutput> Validate(String stepName, Func<TOutput, FlowContext, CancellationToken, Task<Result<Unit, Error>>> validationFunc)
    {
        ArgumentNullException.ThrowIfNull(stepName);
        ArgumentNullException.ThrowIfNull(validationFunc);

        _steps.Add(async (input, context, ct) =>
        {
            if (input is TOutput typedInput)
            {
                var result = await validationFunc(typedInput, context, ct);
                return result.Map(_ => (Object?)typedInput);
            }
            return Error.Create("code", $"Invalid input type for validation step '{stepName}'. Expected {typeof(TOutput).Name}, got {input?.GetType().Name ?? "null"}");
        });

        _stepNames.Add(stepName);
        return this;
    }

    public FlowBuilder<TInput, TOutput> Validate(String stepName, IValidationStep<TOutput> validationStep)
    {
        ArgumentNullException.ThrowIfNull(validationStep);
        return Validate(stepName, async (input, context, ct) =>
        {
            var result = await validationStep.ExecuteAsync(input, context, ct);
            return result.Map(_ => Unit.Value);
        });
    }

    public FlowBuilder<TInput, TOutput> Do(String stepName, Func<TOutput, FlowContext, CancellationToken, Task<Result<Unit, Error>>> actionFunc)
    {
        ArgumentNullException.ThrowIfNull(stepName);
        ArgumentNullException.ThrowIfNull(actionFunc);

        _steps.Add(async (input, context, ct) =>
        {
            if (input is TOutput typedInput)
            {
                var result = await actionFunc(typedInput, context, ct);
                return result.Map(_ => (Object?)typedInput);
            }
            return Error.Create("code", $"Invalid input type for action step '{stepName}'. Expected {typeof(TOutput).Name}, got {input?.GetType().Name ?? "null"}");
        });

        _stepNames.Add(stepName);
        return this;
    }

    public FlowBuilder<TInput, TOutput> Do(String stepName, IFlowStep<TOutput> step)
    {
        ArgumentNullException.ThrowIfNull(step);
        return Do(stepName, step.ExecuteAsync);
    }

    public FlowBuilder<TInput, TOutput> Tap(String stepName, Func<TOutput, FlowContext, Task> tapFunc)
    {
        ArgumentNullException.ThrowIfNull(stepName);
        ArgumentNullException.ThrowIfNull(tapFunc);

        return Do(stepName, async (input, context, ct) =>
        {
            await tapFunc(input, context);
            return Unit.Value;
        });
    }

    public FlowBuilder<TInput, TOutput> When(Func<TOutput, FlowContext, Boolean> condition, Action<FlowBuilder<TInput, TOutput>> configureConditional)
    {
        ArgumentNullException.ThrowIfNull(condition);
        ArgumentNullException.ThrowIfNull(configureConditional);

        var conditionalBuilder = new FlowBuilder<TInput, TOutput>(_name);
        configureConditional(conditionalBuilder);

        _steps.Add(async (input, context, ct) =>
        {
            if (input is TOutput typedInput)
            {
                if (condition(typedInput, context))
                {
                    foreach (var step in conditionalBuilder._steps)
                    {
                        var result = await step(input, context, ct);
                        if (result.IsFailure)
                        {
                            return result;
                        }
                        input = result.Value;
                    }
                }
                return (Object?)input;
            }
            return Error.Create("code", $"Invalid input type for conditional step. Expected {typeof(TOutput).Name}, got {input?.GetType().Name ?? "null"}");
        });

        _stepNames.Add($"When({String.Join(", ", conditionalBuilder._stepNames)})");
        return this;
    }

    public IFlow<TInput, TOutput> Build()
    {
        if (_steps.Count == 0)
        {
            throw new InvalidOperationException("Cannot build a flow with no steps");
        }

        return new Flow<TInput, TOutput>(_name, _steps, _stepNames);
    }
}

public class FlowBuilder<TOutput> where TOutput : notnull
{
    internal readonly String _name;
    internal readonly List<Func<FlowContext, CancellationToken, Task<Result<Object?, Error>>>> _steps = new();
    internal readonly List<String> _stepNames = new();

    internal FlowBuilder(String name)
    {
        _name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public FlowBuilder<TNewOutput> Step<TNewOutput>(String stepName, Func<FlowContext, CancellationToken, Task<Result<TNewOutput, Error>>> stepFunc) where TNewOutput : notnull
    {
        ArgumentNullException.ThrowIfNull(stepName);
        ArgumentNullException.ThrowIfNull(stepFunc);

        var newBuilder = new FlowBuilder<TNewOutput>(_name);

        foreach (var step in _steps)
        {
            newBuilder._steps.Add(step);
        }

        foreach (var name in _stepNames)
        {
            newBuilder._stepNames.Add(name);
        }

        newBuilder._steps.Add(async (context, ct) =>
        {
            var result = await stepFunc(context, ct);
            return result.Map(r => (Object?)r);
        });

        newBuilder._stepNames.Add(stepName);

        return newBuilder;
    }

    public FlowBuilder<TOutput> Do(String stepName, Func<FlowContext, CancellationToken, Task<Result<Unit, Error>>> actionFunc)
    {
        ArgumentNullException.ThrowIfNull(stepName);
        ArgumentNullException.ThrowIfNull(actionFunc);

        _steps.Add(async (context, ct) =>
        {
            var result = await actionFunc(context, ct);
            return result.Map(_ => (Object?)null);
        });

        _stepNames.Add(stepName);
        return this;
    }

    public FlowBuilder<TOutput> Do(String stepName, IFlowStep step)
    {
        ArgumentNullException.ThrowIfNull(step);
        return Do(stepName, step.ExecuteAsync);
    }

    public IFlow<TOutput> Build()
    {
        if (_steps.Count == 0)
        {
            throw new InvalidOperationException("Cannot build a flow with no steps");
        }

        return new Flow<TOutput>(_name, _steps, _stepNames);
    }
}

public class FlowBuilder
{
    private readonly String _name;
    private readonly List<Func<FlowContext, CancellationToken, Task<Result<Unit, Error>>>> _steps = new();
    private readonly List<String> _stepNames = new();

    internal FlowBuilder(String name)
    {
        _name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public static FlowBuilder<TInput, TInput> For<TInput>(String name) where TInput : notnull => new(name);

    public static FlowBuilder Start(String name) => new(name);

    public FlowBuilder<TOutput> Step<TOutput>(String stepName, Func<FlowContext, CancellationToken, Task<Result<TOutput, Error>>> stepFunc) where TOutput : notnull
    {
        ArgumentNullException.ThrowIfNull(stepName);
        ArgumentNullException.ThrowIfNull(stepFunc);

        var newBuilder = new FlowBuilder<TOutput>(_name);

        foreach (var step in _steps)
        {
            newBuilder._steps.Add(async (context, ct) =>
            {
                var result = await step(context, ct);
                return result.Map(_ => (Object?)null);
            });
        }

        foreach (var name in _stepNames)
        {
            newBuilder._stepNames.Add(name);
        }

        newBuilder._steps.Add(async (context, ct) =>
        {
            var result = await stepFunc(context, ct);
            return result.Map(r => (Object?)r);
        });

        newBuilder._stepNames.Add(stepName);

        return newBuilder;
    }

    public FlowBuilder Do(String stepName, Func<FlowContext, CancellationToken, Task<Result<Unit, Error>>> actionFunc)
    {
        ArgumentNullException.ThrowIfNull(stepName);
        ArgumentNullException.ThrowIfNull(actionFunc);

        _steps.Add(actionFunc);
        _stepNames.Add(stepName);
        return this;
    }

    public FlowBuilder Do(String stepName, IFlowStep step)
    {
        ArgumentNullException.ThrowIfNull(step);
        return Do(stepName, step.ExecuteAsync);
    }

    public IFlow Build()
    {
        if (_steps.Count == 0)
        {
            throw new InvalidOperationException("Cannot build a flow with no steps");
        }

        return new Flow(_name, _steps, _stepNames);
    }
}
