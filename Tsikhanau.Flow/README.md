# Tsikhanau.Flow

Workflow execution engine with step-by-step processing, error handling, and execution metadata.

## Features

- **Sequential Execution** - Step-by-step workflow processing
- **Shared Context** - Pass data between steps
- **Error Handling** - Railway-oriented error propagation
- **Metadata Tracking** - Execution time and step information

## Installation

```bash
dotnet add package Tsikhanau.Flow
```

## Usage

```csharp
using Tsikhanau.Flow;

var flow = new FlowBuilder<MyContext>()
    .AddStep(ctx => ValidateInput(ctx))
    .AddStep(ctx => ProcessData(ctx))
    .AddStep(ctx => SaveResult(ctx))
    .Build();

var context = new MyContext { Input = "data" };
var result = await flow.ExecuteAsync(context);
```

## License

MIT
