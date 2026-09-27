namespace Tsikhanau.Foundation.General;

public static class Empty
{
    public const String String = "";

    public static readonly Guid Guid = Guid.Empty;

    public static T[] Array<T>() => [];
}