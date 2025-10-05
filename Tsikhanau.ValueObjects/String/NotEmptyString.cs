using Tsikhanau.Outcomes;
using Tsikhanau.Outcomes.Errors;
using Tsikhanau.Outcomes.Result;

namespace Tsikhanau.Packages.ValueObjects.String;

public readonly record struct NotEmptyString
{
    public readonly System.String Value { get; }

    private NotEmptyString(System.String value)
    {
        this.Value = value;
    }

    public static Result<NotEmptyString, Error> FromString(System.String value)
    {
        if (!System.String.IsNullOrWhiteSpace(value))
            return Result.Success(new NotEmptyString(value));

        return Result<NotEmptyString, Error>.Failure("String cannot be null, empty or whitespace.");
    }
}