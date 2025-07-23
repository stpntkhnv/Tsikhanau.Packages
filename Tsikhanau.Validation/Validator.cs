using Tsikhanau.Foundation.Validation;

namespace Tsikhanau.Validation;

public sealed class Validator<T>
{
    private readonly T Data;
    
    private Validator(T data)
    {
        Data = data;
    }

    public static Validator<T> ForObject(T data)
    {
        Guard.AgainstNull(data);
        return new Validator<T>(data);
    }
}