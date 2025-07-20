namespace Tsikhanau.Foundation.DateTime;

public sealed class SystemClock : IClock
{
    public System.DateTime UtcNow => System.DateTime.UtcNow;
}