using Tsikhanau.Monads.Errors;
using Tsikhanau.Monads.Result;

namespace Tsikhanau.Validation.Abstractions;

public interface IValidator<T> where T : notnull
{
    Result<T, Error> Validate(T instance);
}
