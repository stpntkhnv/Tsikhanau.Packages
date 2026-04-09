# Tsikhanau.Flow

Step-by-step workflow engine built on top of `Result<T, Error>` monads.

## When to use

Use Flow when you need named steps, execution tracking, or retry/timeout on specific steps.
For simple chains, `Bind`/`Map`/`Tap` from RailwayExtensions is enough.

## Usage

```csharp
using Tsikhanau.Flow;

var flow = FlowBuilder.For<CreateOrderCommand>("CreateOrder")
    .Step<Order>("validate", (cmd, ctx, ct) => ValidateOrder(cmd))
    .Step<EnrichedOrder>("enrich", (order, ctx, ct) => EnrichWithPricing(order))
    .Validate("check-total", (order, ctx, ct) =>
        order.Total > 0
            ? Task.FromResult(Result.Success())
            : Task.FromResult<Result<Unit, Error>>(Error.Create("validation", "total must be > 0")))
    .Step<SavedOrder>("save", (order, ctx, ct) => SaveToDatabase(order))
    .Tap("notify", (order, ctx) => SendNotification(order))
    .Build();

var result = await flow.ExecuteAsync(command);
```

### Retry

```csharp
var flow = FlowBuilder.For<OrderCommand>("PlaceOrder")
    .Step<Order>("validate", (cmd, ctx, ct) => Validate(cmd))
    .WithRetry("send-email", SendConfirmation, maxAttempts: 3, delay: TimeSpan.FromSeconds(1))
    .Build();
```

### Conditional steps

```csharp
var flow = FlowBuilder.For<Order>("ProcessOrder")
    .When((order, ctx) => order.IsExpress, builder => builder
        .Do("priority-queue", (order, ctx, ct) => EnqueuePriority(order)))
    .Step<Receipt>("finalize", (order, ctx, ct) => Finalize(order))
    .Build();
```

### Shared context

Steps can share data through `FlowContext`:

```csharp
.Do("cache-user", async (order, ctx, ct) =>
{
    var user = await LoadUser(order.UserId, ct);
    ctx.Set("user", user);
    return Result.Success();
})
.Tap("log", (order, ctx) =>
{
    var user = ctx.Get<User>("user");
    logger.LogInformation("order for {Name}", user.Name);
})
```

### Execution metadata

After execution, `FlowContext` contains step timing and status info via `FlowExecutionContext`.

## License

MIT
