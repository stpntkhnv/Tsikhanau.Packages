using Tsikhanau.Foundation.General;
using Tsikhanau.Monads.Errors;
using Tsikhanau.Monads.Result;

namespace Tsikhanau.Validation.Abstractions;

public interface IFieldValidator<in TObject>
{
    Result<Unit, Error> Validate(TObject obj);
    Task<Result<Unit, Error>> ValidateAsync(TObject obj);
}