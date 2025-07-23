using Tsikhanau.Foundation.General;
using Tsikhanau.Outcomes;
using Tsikhanau.Outcomes.Errors;
using Tsikhanau.Outcomes.Result;
using Tsikhanau.RailwayExtensions;
using Tsikhanau.RailwayExtensions.Result;
using Tsikhanau.RailwayExtensions.Result.Map;

namespace Tsikhanau.Flow;

public abstract class FlowStepBase : IFlowStep
{
    protected FlowStepBase(String name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public String Name { get; }

    public async Task<Result<Unit, Error>> ExecuteAsync(FlowContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            return await ExecuteInternalAsync(context, cancellationToken);
        }
        catch (Exception ex) when (!(ex is OperationCanceledException))
        {
            return Error.FromException(ex);
        }
    }

    protected abstract Task<Result<Unit, Error>> ExecuteInternalAsync(FlowContext context, CancellationToken cancellationToken);
}

public abstract class FlowStepBase<TInput> : IFlowStep<TInput>
{
    protected FlowStepBase(String name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public String Name { get; }

    public async Task<Result<Unit, Error>> ExecuteAsync(TInput input, FlowContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            return await ExecuteInternalAsync(input, context, cancellationToken);
        }
        catch (Exception ex) when (!(ex is OperationCanceledException))
        {
            return Error.FromException(ex);
        }
    }

    protected abstract Task<Result<Unit, Error>> ExecuteInternalAsync(TInput input, FlowContext context, CancellationToken cancellationToken);
}

public abstract class TransformStepBase<TInput, TOutput> : ITransformStep<TInput, TOutput>
{
    protected TransformStepBase(String name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public String Name { get; }

    public async Task<Result<TOutput, Error>> ExecuteAsync(TInput input, FlowContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            return await ExecuteInternalAsync(input, context, cancellationToken);
        }
        catch (Exception ex) when (!(ex is OperationCanceledException))
        {
            return Error.FromException(ex);
        }
    }

    protected abstract Task<Result<TOutput, Error>> ExecuteInternalAsync(TInput input, FlowContext context, CancellationToken cancellationToken);
}

public abstract class ValidationStepBase<TInput> : IValidationStep<TInput>
{
    protected ValidationStepBase(String name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public String Name { get; }

    public async Task<Result<TInput, Error>> ExecuteAsync(TInput input, FlowContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var validationResult = await ValidateInternalAsync(input, context, cancellationToken);
            return validationResult.Map(_ => input);
        }
        catch (Exception ex) when (!(ex is OperationCanceledException))
        {
            return Error.FromException(ex);
        }
    }

    protected abstract Task<Result<Unit, Error>> ValidateInternalAsync(TInput input, FlowContext context, CancellationToken cancellationToken);
}