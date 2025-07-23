using Tsikhanau.Foundation.General;
using Tsikhanau.Outcomes.Errors;
using Tsikhanau.Outcomes.Result;

namespace Tsikhanau.Validation.Abstractions;

public interface IFieldValidator<in TObject>
{
    Result<Unit, ValidationError> Validate(TObject obj);
}