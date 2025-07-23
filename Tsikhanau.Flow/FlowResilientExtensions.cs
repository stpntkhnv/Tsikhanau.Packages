using Tsikhanau.Foundation.General;
using Tsikhanau.Outcomes;
using Tsikhanau.Outcomes.Errors;
using Tsikhanau.Outcomes.Result;
using Tsikhanau.RailwayExtensions;

namespace Tsikhanau.Flow;

public static class FlowResilientExtensions
{
    public static FlowBuilder<TInput, TOutput> WithRetry<TInput, TOutput>(
        this FlowBuilder<TInput, TOutput> builder,
        String stepName,
        Func<TOutput, FlowContext, CancellationToken, Task<Result<TOutput, Error>>> operation,
        Int32 maxAttempts = 3,
        TimeSpan? delay = null,
        Func<Error, Int32, Boolean>? shouldRetry = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(stepName);
        ArgumentNullException.ThrowIfNull(operation);

        if (maxAttempts <= 0)
            throw new ArgumentException("Max attempts must be greater than 0", nameof(maxAttempts));

        var retryDelay = delay ?? TimeSpan.FromMilliseconds(100);
        shouldRetry ??= (_, _) => true;

        return builder.Step(stepName, async (input, context, ct) =>
        {
            Error? lastError = null;

            for (Int32 attempt = 1; attempt <= maxAttempts; attempt++)
            {
                var result = await operation(input, context, ct);
                
                if (result.IsSuccess)
                {
                    if (attempt > 1)
                    {
                        context.SetMetadata($"{stepName}_retry_attempts", attempt);
                        context.SetMetadata($"{stepName}_retry_successful", true);
                    }
                    return result;
                }

                lastError = result.Error;

                if (attempt == maxAttempts || !shouldRetry(result.Error, attempt))
                {
                    break;
                }

                if (retryDelay > TimeSpan.Zero)
                {
                    await Task.Delay(retryDelay, ct);
                }
            }

            context.SetMetadata($"{stepName}_retry_attempts", maxAttempts);
            context.SetMetadata($"{stepName}_retry_successful", false);
            context.SetMetadata($"{stepName}_retry_last_error", lastError);

            return lastError!.WithContext("code", $"Step '{stepName}' failed after {maxAttempts} attempts");
        });
    }

    public static FlowBuilder<TInput, TOutput> WithCircuitBreaker<TInput, TOutput>(
        this FlowBuilder<TInput, TOutput> builder,
        String stepName,
        Func<TOutput, FlowContext, CancellationToken, Task<Result<TOutput, Error>>> operation,
        Int32 failureThreshold = 5,
        TimeSpan? timeout = null,
        TimeSpan? resetTimeout = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(stepName);
        ArgumentNullException.ThrowIfNull(operation);

        if (failureThreshold <= 0)
            throw new ArgumentException("Failure threshold must be greater than 0", nameof(failureThreshold));

        var circuitTimeout = timeout ?? TimeSpan.FromSeconds(30);
        var circuitResetTimeout = resetTimeout ?? TimeSpan.FromMinutes(1);

        return builder.Step(stepName, async (input, context, ct) =>
        {
            var circuitBreakerKey = $"__circuit_breaker_{stepName}";
            var circuitBreaker = context.GetOrDefault<CircuitBreakerState>(circuitBreakerKey, new CircuitBreakerState())!;

            context.Set(circuitBreakerKey, circuitBreaker);

            if (circuitBreaker.State == CircuitState.Open)
            {
                if (DateTime.UtcNow - circuitBreaker.LastFailureTime < circuitResetTimeout)
                {
                    context.SetMetadata($"{stepName}_circuit_breaker_state", "open");
                    return Error.Create("code", $"Circuit breaker is open for step '{stepName}'");
                }
                circuitBreaker.State = CircuitState.HalfOpen;
            }

            using var timeoutCts = new CancellationTokenSource(circuitTimeout);
            using var combinedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, timeoutCts.Token);

            try
            {
                var result = await operation(input, context, combinedCts.Token);

                if (result.IsSuccess)
                {
                    circuitBreaker.FailureCount = 0;
                    circuitBreaker.State = CircuitState.Closed;
                    context.SetMetadata($"{stepName}_circuit_breaker_state", "closed");
                    return result;
                }

                circuitBreaker.FailureCount++;
                circuitBreaker.LastFailureTime = DateTime.UtcNow;

                if (circuitBreaker.FailureCount >= failureThreshold)
                {
                    circuitBreaker.State = CircuitState.Open;
                    context.SetMetadata($"{stepName}_circuit_breaker_state", "open");
                }
                else
                {
                    context.SetMetadata($"{stepName}_circuit_breaker_state", "closed");
                }

                context.SetMetadata($"{stepName}_circuit_breaker_failures", circuitBreaker.FailureCount);
                return result;
            }
            catch (OperationCanceledException) when (timeoutCts.Token.IsCancellationRequested)
            {
                circuitBreaker.FailureCount++;
                circuitBreaker.LastFailureTime = DateTime.UtcNow;

                if (circuitBreaker.FailureCount >= failureThreshold)
                {
                    circuitBreaker.State = CircuitState.Open;
                }

                context.SetMetadata($"{stepName}_circuit_breaker_state", circuitBreaker.State.ToString().ToLowerInvariant());
                context.SetMetadata($"{stepName}_circuit_breaker_failures", circuitBreaker.FailureCount);

                return Error.Create("code", $"Step '{stepName}' timed out after {circuitTimeout}");
            }
        });
    }

    public static FlowBuilder<TInput, TOutput> WithTimeout<TInput, TOutput>(
        this FlowBuilder<TInput, TOutput> builder,
        String stepName,
        Func<TOutput, FlowContext, CancellationToken, Task<Result<TOutput, Error>>> operation,
        TimeSpan timeout)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(stepName);
        ArgumentNullException.ThrowIfNull(operation);

        if (timeout <= TimeSpan.Zero)
            throw new ArgumentException("Timeout must be greater than zero", nameof(timeout));

        return builder.Step(stepName, async (input, context, ct) =>
        {
            using var timeoutCts = new CancellationTokenSource(timeout);
            using var combinedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, timeoutCts.Token);

            try
            {
                return await operation(input, context, combinedCts.Token);
            }
            catch (OperationCanceledException) when (timeoutCts.Token.IsCancellationRequested)
            {
                context.SetMetadata($"{stepName}_timeout", timeout);
                return Error.Create("code", $"Step '{stepName}' timed out after {timeout}");
            }
        });
    }

    public static FlowBuilder<TInput, TOutput> WithFallback<TInput, TOutput>(
        this FlowBuilder<TInput, TOutput> builder,
        String stepName,
        Func<TOutput, FlowContext, CancellationToken, Task<Result<TOutput, Error>>> primaryOperation,
        Func<TOutput, Error, FlowContext, CancellationToken, Task<Result<TOutput, Error>>> fallbackOperation)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(stepName);
        ArgumentNullException.ThrowIfNull(primaryOperation);
        ArgumentNullException.ThrowIfNull(fallbackOperation);

        return builder.Step(stepName, async (input, context, ct) =>
        {
            var primaryResult = await primaryOperation(input, context, ct);
            
            if (primaryResult.IsSuccess)
            {
                context.SetMetadata($"{stepName}_fallback_used", false);
                return primaryResult;
            }

            context.SetMetadata($"{stepName}_fallback_used", true);
            context.SetMetadata($"{stepName}_primary_error", primaryResult.Error);

            var fallbackResult = await fallbackOperation(input, primaryResult.Error, context, ct);
            
            if (fallbackResult.IsFailure)
            {
                return fallbackResult.Error.WithContext("code", $"Both primary and fallback operations failed for step '{stepName}'");
            }

            return fallbackResult;
        });
    }
}

internal class CircuitBreakerState
{
    public CircuitState State { get; set; } = CircuitState.Closed;
    public Int32 FailureCount { get; set; } = 0;
    public DateTime LastFailureTime { get; set; } = DateTime.MinValue;
}

internal enum CircuitState
{
    Closed,
    Open,
    HalfOpen
}