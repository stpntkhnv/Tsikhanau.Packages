using Tsikhanau.Foundation.General;
using Tsikhanau.Outcomes;
using Tsikhanau.Outcomes.Errors;
using Tsikhanau.Outcomes.Result;
using Tsikhanau.RailwayExtensions;

namespace Tsikhanau.Flow;

internal class Flow<TInput, TOutput> : IFlow<TInput, TOutput>
{
    private readonly List<Func<Object?, FlowContext, CancellationToken, Task<Result<Object?, Error>>>> _steps;
    private readonly List<String> _stepNames;

    internal Flow(String name, List<Func<Object?, FlowContext, CancellationToken, Task<Result<Object?, Error>>>> steps, List<String> stepNames)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        _steps = steps ?? throw new ArgumentNullException(nameof(steps));
        _stepNames = stepNames ?? throw new ArgumentNullException(nameof(stepNames));
    }

    public String Name { get; }
    public IReadOnlyList<String> StepNames => _stepNames.AsReadOnly();

    public Task<Result<TOutput, Error>> ExecuteAsync(TInput input, CancellationToken cancellationToken = default)
    {
        return ExecuteAsync(input, new FlowContext(), cancellationToken);
    }

    public async Task<Result<TOutput, Error>> ExecuteAsync(TInput input, FlowContext context, CancellationToken cancellationToken = default)
    {
        var executionContext = new FlowExecutionContext
        {
            FlowName = Name
        };

        context.SetMetadata("__execution_context", executionContext);

        Object? currentValue = input;

        for (Int32 i = 0; i < _steps.Count; i++)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                executionContext.IsCancelled = true;
                return Error.Create("code", $"Flow '{Name}' was cancelled at step '{_stepNames[i]}'");
            }

            executionContext.CurrentStepIndex = i;
            executionContext.CurrentStepName = _stepNames[i];
            executionContext.CurrentStepStartTime = DateTime.UtcNow;

            var stepResult = await _steps[i](currentValue, context, cancellationToken);

            var stepInfo = new StepExecutionInfo
            {
                StepName = _stepNames[i],
                StepIndex = i,
                StartTime = executionContext.CurrentStepStartTime,
                EndTime = DateTime.UtcNow,
                IsSuccessful = stepResult.IsSuccess,
                Error = stepResult.IsFailure ? stepResult.Error : null
            };

            executionContext.CompletedSteps.Add(stepInfo);

            if (stepResult.IsFailure)
            {
                executionContext.HasFailed = true;
                executionContext.LastError = stepResult.Error;
                return stepResult.Error.WithContext("code", $"Flow '{Name}' failed at step '{_stepNames[i]}'");
            }

            currentValue = stepResult.Value;
        }

        if (currentValue is TOutput output)
        {
            return output;
        }

        return Error.Create("code", $"Flow '{Name}' completed but final result is not of expected type {typeof(TOutput).Name}");
    }
}

internal class Flow<TOutput> : IFlow<TOutput>
{
    private readonly List<Func<FlowContext, CancellationToken, Task<Result<Object?, Error>>>> _steps;
    private readonly List<String> _stepNames;

    internal Flow(String name, List<Func<FlowContext, CancellationToken, Task<Result<Object?, Error>>>> steps, List<String> stepNames)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        _steps = steps ?? throw new ArgumentNullException(nameof(steps));
        _stepNames = stepNames ?? throw new ArgumentNullException(nameof(stepNames));
    }

    public String Name { get; }
    public IReadOnlyList<String> StepNames => _stepNames.AsReadOnly();

    public Task<Result<TOutput, Error>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        return ExecuteAsync(new FlowContext(), cancellationToken);
    }

    public async Task<Result<TOutput, Error>> ExecuteAsync(FlowContext context, CancellationToken cancellationToken = default)
    {
        var executionContext = new FlowExecutionContext
        {
            FlowName = Name
        };

        context.SetMetadata("__execution_context", executionContext);

        Object? currentValue = null;

        for (Int32 i = 0; i < _steps.Count; i++)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                executionContext.IsCancelled = true;
                return Error.Create("code", $"Flow '{Name}' was cancelled at step '{_stepNames[i]}'");
            }

            executionContext.CurrentStepIndex = i;
            executionContext.CurrentStepName = _stepNames[i];
            executionContext.CurrentStepStartTime = DateTime.UtcNow;

            var stepResult = await _steps[i](context, cancellationToken);

            var stepInfo = new StepExecutionInfo
            {
                StepName = _stepNames[i],
                StepIndex = i,
                StartTime = executionContext.CurrentStepStartTime,
                EndTime = DateTime.UtcNow,
                IsSuccessful = stepResult.IsSuccess,
                Error = stepResult.IsFailure ? stepResult.Error : null
            };

            executionContext.CompletedSteps.Add(stepInfo);

            if (stepResult.IsFailure)
            {
                executionContext.HasFailed = true;
                executionContext.LastError = stepResult.Error;
                return stepResult.Error.WithContext("code", $"Flow '{Name}' failed at step '{_stepNames[i]}'");
            }

            currentValue = stepResult.Value;
        }

        if (currentValue is TOutput output)
        {
            return output;
        }

        return Error.Create("code", $"Flow '{Name}' completed but final result is not of expected type {typeof(TOutput).Name}");
    }
}

internal class Flow : IFlow
{
    private readonly List<Func<FlowContext, CancellationToken, Task<Result<Unit, Error>>>> _steps;
    private readonly List<String> _stepNames;

    internal Flow(String name, List<Func<FlowContext, CancellationToken, Task<Result<Unit, Error>>>> steps, List<String> stepNames)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        _steps = steps ?? throw new ArgumentNullException(nameof(steps));
        _stepNames = stepNames ?? throw new ArgumentNullException(nameof(stepNames));
    }

    public String Name { get; }
    public IReadOnlyList<String> StepNames => _stepNames.AsReadOnly();

    public Task<Result<Unit, Error>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        return ExecuteAsync(new FlowContext(), cancellationToken);
    }

    public async Task<Result<Unit, Error>> ExecuteAsync(FlowContext context, CancellationToken cancellationToken = default)
    {
        var executionContext = new FlowExecutionContext
        {
            FlowName = Name
        };

        context.SetMetadata("__execution_context", executionContext);

        for (Int32 i = 0; i < _steps.Count; i++)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                executionContext.IsCancelled = true;
                return Error.Create("code", $"Flow '{Name}' was cancelled at step '{_stepNames[i]}'");
            }

            executionContext.CurrentStepIndex = i;
            executionContext.CurrentStepName = _stepNames[i];
            executionContext.CurrentStepStartTime = DateTime.UtcNow;

            var stepResult = await _steps[i](context, cancellationToken);

            var stepInfo = new StepExecutionInfo
            {
                StepName = _stepNames[i],
                StepIndex = i,
                StartTime = executionContext.CurrentStepStartTime,
                EndTime = DateTime.UtcNow,
                IsSuccessful = stepResult.IsSuccess,
                Error = stepResult.IsFailure ? stepResult.Error : null
            };

            executionContext.CompletedSteps.Add(stepInfo);

            if (stepResult.IsFailure)
            {
                executionContext.HasFailed = true;
                executionContext.LastError = stepResult.Error;
                return stepResult.Error.WithContext("code", $"Flow '{Name}' failed at step '{_stepNames[i]}'");
            }
        }

        return Unit.Value;
    }
}