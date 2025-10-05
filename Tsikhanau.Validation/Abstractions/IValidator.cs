using Tsikhanau.Outcomes.Errors;
using Tsikhanau.Outcomes.Result;

namespace Tsikhanau.Validation.Abstractions;

public interface IValidator<T>
{
    Result<T, ValidationError> Validate(T instance);
}