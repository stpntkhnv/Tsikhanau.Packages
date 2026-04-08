using Tsikhanau.Foundation.General;
using Tsikhanau.Monads;
using Tsikhanau.Monads.Errors;
using Tsikhanau.Monads.Result;

namespace Tsikhanau.Flow;

public interface IConditionalFlow<TInput, TOutput>
{
    IConditionalFlow<TInput, TOutput> When(Func<TInput, FlowContext, Boolean> condition, IFlow<TInput, TOutput> flow);
    IConditionalFlow<TInput, TOutput> When(Func<TInput, FlowContext, Boolean> condition, Func<IFlow<TInput, TOutput>> flowFactory);
    IConditionalFlow<TInput, TOutput> Otherwise(IFlow<TInput, TOutput> flow);
    IConditionalFlow<TInput, TOutput> Otherwise(Func<IFlow<TInput, TOutput>> flowFactory);
    Task<Result<TOutput, Error>> ExecuteAsync(TInput input, FlowContext context, CancellationToken cancellationToken = default);
}

public class ConditionalFlow<TInput, TOutput> : IConditionalFlow<TInput, TOutput>
{
    private readonly List<(Func<TInput, FlowContext, Boolean> Condition, Func<IFlow<TInput, TOutput>> FlowFactory)> _conditionalFlows = new();
    private Func<IFlow<TInput, TOutput>>? _otherwiseFlowFactory;

    public IConditionalFlow<TInput, TOutput> When(Func<TInput, FlowContext, Boolean> condition, IFlow<TInput, TOutput> flow)
    {
        ArgumentNullException.ThrowIfNull(condition);
        ArgumentNullException.ThrowIfNull(flow);
        
        _conditionalFlows.Add((condition, () => flow));
        return this;
    }

    public IConditionalFlow<TInput, TOutput> When(Func<TInput, FlowContext, Boolean> condition, Func<IFlow<TInput, TOutput>> flowFactory)
    {
        ArgumentNullException.ThrowIfNull(condition);
        ArgumentNullException.ThrowIfNull(flowFactory);
        
        _conditionalFlows.Add((condition, flowFactory));
        return this;
    }

    public IConditionalFlow<TInput, TOutput> Otherwise(IFlow<TInput, TOutput> flow)
    {
        ArgumentNullException.ThrowIfNull(flow);
        
        _otherwiseFlowFactory = () => flow;
        return this;
    }

    public IConditionalFlow<TInput, TOutput> Otherwise(Func<IFlow<TInput, TOutput>> flowFactory)
    {
        ArgumentNullException.ThrowIfNull(flowFactory);
        
        _otherwiseFlowFactory = flowFactory;
        return this;
    }

    public async Task<Result<TOutput, Error>> ExecuteAsync(TInput input, FlowContext context, CancellationToken cancellationToken = default)
    {
        foreach (var (condition, flowFactory) in _conditionalFlows)
        {
            if (condition(input, context))
            {
                var flow = flowFactory();
                return await flow.ExecuteAsync(input, context, cancellationToken);
            }
        }

        if (_otherwiseFlowFactory != null)
        {
            var otherwiseFlow = _otherwiseFlowFactory();
            return await otherwiseFlow.ExecuteAsync(input, context, cancellationToken);
        }

        return Error.Create("code", "No matching condition found and no otherwise flow specified");
    }
}

public static class ConditionalFlowExtensions
{
    public static IConditionalFlow<TInput, TOutput> Branch<TInput, TOutput>(this FlowBuilder<TInput, TOutput> builder)
    {
        return new ConditionalFlow<TInput, TOutput>();
    }
}

public interface IParallelFlow<TInput, TOutput>
{
    IParallelFlow<TInput, TOutput> Add(String name, IFlow<TInput, TOutput> flow);
    IParallelFlow<TInput, TOutput> Add(String name, Func<IFlow<TInput, TOutput>> flowFactory);
    Task<Result<IReadOnlyDictionary<String, TOutput>, Error>> ExecuteAsync(TInput input, FlowContext context, CancellationToken cancellationToken = default);
}

public class ParallelFlow<TInput, TOutput> : IParallelFlow<TInput, TOutput>
{
    private readonly List<(String Name, Func<IFlow<TInput, TOutput>> FlowFactory)> _parallelFlows = new();

    public IParallelFlow<TInput, TOutput> Add(String name, IFlow<TInput, TOutput> flow)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(flow);
        
        _parallelFlows.Add((name, () => flow));
        return this;
    }

    public IParallelFlow<TInput, TOutput> Add(String name, Func<IFlow<TInput, TOutput>> flowFactory)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(flowFactory);
        
        _parallelFlows.Add((name, flowFactory));
        return this;
    }

    public async Task<Result<IReadOnlyDictionary<String, TOutput>, Error>> ExecuteAsync(TInput input, FlowContext context, CancellationToken cancellationToken = default)
    {
        if (_parallelFlows.Count == 0)
        {
            return new Dictionary<String, TOutput>().AsReadOnly();
        }

        var tasks = _parallelFlows.Select(async (flowInfo) =>
        {
            var childContext = new FlowContext(context);
            var flow = flowInfo.FlowFactory();
            var result = await flow.ExecuteAsync(input, childContext, cancellationToken);
            return (flowInfo.Name, Result: result);
        });

        var results = await Task.WhenAll(tasks);
        var resultDict = new Dictionary<String, TOutput>();
        
        foreach (var (name, result) in results)
        {
            if (result.IsFailure)
            {
                return result.Error.WithContext("code", $"Parallel flow '{name}' failed");
            }
            resultDict[name] = result.Value;
        }

        return resultDict.AsReadOnly();
    }
}

public static class ParallelFlowExtensions
{
    public static IParallelFlow<TInput, TOutput> Parallel<TInput, TOutput>()
    {
        return new ParallelFlow<TInput, TOutput>();
    }
}