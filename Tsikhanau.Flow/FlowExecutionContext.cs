using Tsikhanau.Monads;
using Tsikhanau.Monads.Errors;

namespace Tsikhanau.Flow;

public class FlowExecutionContext
{
    public String FlowName { get; init; } = String.Empty;
    public String CurrentStepName { get; set; } = String.Empty;
    public Int32 CurrentStepIndex { get; set; }
    public DateTime StartTime { get; init; } = DateTime.UtcNow;
    public DateTime CurrentStepStartTime { get; set; } = DateTime.UtcNow;
    public List<StepExecutionInfo> CompletedSteps { get; } = new();
    public Boolean IsCancelled { get; set; }
    public Boolean HasFailed { get; set; }
    public Error? LastError { get; set; }
}

public class StepExecutionInfo
{
    public String StepName { get; init; } = String.Empty;
    public Int32 StepIndex { get; init; }
    public DateTime StartTime { get; init; }
    public DateTime EndTime { get; init; }
    public TimeSpan Duration => EndTime - StartTime;
    public Boolean IsSuccessful { get; init; }
    public Error? Error { get; init; }
    public Dictionary<String, Object> Metadata { get; init; } = new();
}