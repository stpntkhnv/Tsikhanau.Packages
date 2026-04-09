using Tsikhanau.Foundation.General;
using Tsikhanau.Monads;
using Tsikhanau.Monads.Errors;
using Tsikhanau.Monads.Result;

namespace Tsikhanau.Flow;

public interface IFlowStep
{
    String Name { get; }
    Task<Result<Unit, Error>> ExecuteAsync(FlowContext context, CancellationToken cancellationToken = default);
}

public interface IFlowStep<TInput> where TInput : notnull
{
    String Name { get; }
    Task<Result<Unit, Error>> ExecuteAsync(TInput input, FlowContext context, CancellationToken cancellationToken = default);
}

public interface ITransformStep<TInput, TOutput> where TInput : notnull where TOutput : notnull
{
    String Name { get; }
    Task<Result<TOutput, Error>> ExecuteAsync(TInput input, FlowContext context, CancellationToken cancellationToken = default);
}

public interface IValidationStep<TInput> where TInput : notnull
{
    String Name { get; }
    Task<Result<TInput, Error>> ExecuteAsync(TInput input, FlowContext context, CancellationToken cancellationToken = default);
}