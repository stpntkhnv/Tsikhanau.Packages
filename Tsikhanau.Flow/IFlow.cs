using Tsikhanau.Foundation.General;
using Tsikhanau.Outcomes;
using Tsikhanau.Outcomes.Errors;
using Tsikhanau.Outcomes.Result;

namespace Tsikhanau.Flow;

public interface IFlow<TInput, TOutput>
{
    String Name { get; }
    IReadOnlyList<String> StepNames { get; }
    Task<Result<TOutput, Error>> ExecuteAsync(TInput input, CancellationToken cancellationToken = default);
    Task<Result<TOutput, Error>> ExecuteAsync(TInput input, FlowContext context, CancellationToken cancellationToken = default);
}

public interface IFlow<TOutput>
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