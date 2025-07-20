namespace Tsikhanau.Foundation.DateTime;

public interface IClock
{
    System.DateTime UtcNow { get; }

    System.DateTime Now => UtcNow.ToLocalTime();
}