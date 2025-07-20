namespace Tsikhanau.Foundation.DateTime;

public sealed class FakeClock(System.DateTime startTime) : IClock
{
    private System.DateTime _currentTime = startTime.Kind == DateTimeKind.Utc ? startTime : startTime.ToUniversalTime();

    public FakeClock() : this(System.DateTime.UtcNow)
    {
    }

    public System.DateTime UtcNow => _currentTime;

    public void AdvanceBy(TimeSpan duration)
    {
        _currentTime = _currentTime.Add(duration);
    }

    public void SetTime(System.DateTime time)
    {
        _currentTime = time.Kind == DateTimeKind.Utc ? time : time.ToUniversalTime();
    }
}