using Tsikhanau.Foundation.General;
using Tsikhanau.Monads;
using Tsikhanau.Monads.Errors;
using Tsikhanau.Monads.Result;
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
        Func<Error, Int32, Boolean>? shouldRetry = null) where TInput : notnull where TOutput : notnull
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

    public static FlowBuilder<TInput, TOutput> WithTimeout<TInput, TOutput>(
        this FlowBuilder<TInput, TOutput> builder,
        String stepName,
        Func<TOutput, FlowContext, CancellationToken, Task<Result<TOutput, Error>>> operation,
        TimeSpan timeout) where TInput : notnull where TOutput : notnull
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
        Func<TOutput, Error, FlowContext, CancellationToken, Task<Result<TOutput, Error>>> fallbackOperation) where TInput : notnull where TOutput : notnull
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

