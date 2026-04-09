using Tsikhanau.Foundation.General;
using Tsikhanau.Monads;
using Tsikhanau.Monads.Errors;
using Tsikhanau.Monads.Result;

namespace Tsikhanau.Flow;

public interface IFlow<TInput, TOutput> where TInput : notnull where TOutput : notnull
{
    String Name { get; }
    IReadOnlyList<String> StepNames { get; }
    Task<Result<TOutput, Error>> ExecuteAsync(TInput input, CancellationToken cancellationToken = default);
    Task<Result<TOutput, Error>> ExecuteAsync(TInput input, FlowContext context, CancellationToken cancellationToken = default);
}

public interface IFlow<TOutput> where TOutput : notnull
{
    String Name { get; }
    IReadOnlyList<String> StepNames { get; }
    Task<Result<TOutput, Error>> ExecuteAsync(CancellationToken cancellationToken = default);
    Task<Result<TOutput, Error>> ExecuteAsync(FlowContext context, CancellationToken cancellationToken = default);
}

public interface IFlow
{
    String Name { get; }
    IReadOnlyList<String> StepNames { get; }
    Task<Result<Unit, Error>> ExecuteAsync(CancellationToken cancellationToken = default);
    Task<Result<Unit, Error>> ExecuteAsync(FlowContext context, CancellationToken cancellationToken = default);
}