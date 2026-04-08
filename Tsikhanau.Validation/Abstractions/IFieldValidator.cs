using Tsikhanau.Foundation.General;
using Tsikhanau.Monads.Errors;
using Tsikhanau.Monads.Result;

namespace Tsikhanau.Validation.Abstractions;

public interface IFieldValidator<in TObject>
{
    Result<Unit, ValidationError> Validate(TObject obj);
    Task<Result<Unit, ValidationError>> ValidateAsync(TObject obj);
}