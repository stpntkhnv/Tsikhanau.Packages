namespace Tsikhanau.Railway.Tests;

public class ValidationErrorTests
{
    [Fact]
    public void Validation_FieldAndMessage_ReturnsValidationError()
    {
        var error = Error.Validation("name", "Required");

        error.ShouldBeOfType<ValidationError>();
        error.Kind.ShouldBe(ErrorKind.Validation);
        error.Code.ShouldBe("validation");
        error.Message.ShouldBe("name: Required");
        error.FieldErrors.ShouldBe([new FieldError("name", "Required")]);
        error.Inner.ShouldBeNull();
        error.Metadata.ShouldBeNull();
    }

    [Fact]
    public void Validation_NullField_ThrowsArgumentNullException()
    {
        var exception = Should.Throw<ArgumentNullException>(() => Error.Validation(null!, "Required"));

        exception.ParamName.ShouldBe("field");
    }

    [Fact]
    public void For_FieldAndMessage_ReturnsSingleFieldError()
    {
        var error = ValidationError.For("name", "Required");

        error.Kind.ShouldBe(ErrorKind.Validation);
        error.Code.ShouldBe(ValidationError.DefaultCode);
        error.Message.ShouldBe("name: Required");
        error.FieldErrors.ShouldBe([new FieldError("name", "Required")]);
    }

    [Fact]
    public void For_EmptyField_UsesMessageOnly()
    {
        var error = ValidationError.For("", "Object is invalid");

        error.Message.ShouldBe("Object is invalid");
        error.FieldErrors.ShouldBe([new FieldError("", "Object is invalid")]);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void For_EmptyOrWhiteSpaceMessage_ThrowsArgumentException(String message)
    {
        var exception = Should.Throw<ArgumentException>(() => ValidationError.For("name", message));

        exception.ShouldBeOfType<ArgumentException>();
        exception.ParamName.ShouldBe("message");
    }

    [Fact]
    public void From_FieldErrors_KeepsOrderAndJoinsMessage()
    {
        FieldError[] fieldErrors =
        [
            new("name", "Required"),
            new("", "Object is invalid"),
            new("age", "Too young")
        ];

        var error = ValidationError.From(fieldErrors);

        error.Kind.ShouldBe(ErrorKind.Validation);
        error.Code.ShouldBe("validation");
        error.FieldErrors.ShouldBe(fieldErrors);
        error.Message.ShouldBe("name: Required; Object is invalid; age: Too young");
    }

    [Fact]
    public void From_SourceChangedAfterwards_KeepsOriginalFieldErrors()
    {
        var source = new List<FieldError> { new("name", "Required") };

        var error = ValidationError.From(source);
        source.Add(new FieldError("age", "Too young"));

        error.FieldErrors.ShouldBe([new FieldError("name", "Required")]);
    }

    [Fact]
    public void From_Empty_ThrowsArgumentException()
    {
        var exception = Should.Throw<ArgumentException>(() => ValidationError.From([]));

        exception.ShouldBeOfType<ArgumentException>();
        exception.ParamName.ShouldBe("fieldErrors");
    }

    [Fact]
    public void From_DefaultFieldError_ThrowsArgumentException()
    {
        FieldError[] fieldErrors = [new("name", "Required"), default];

        var exception = Should.Throw<ArgumentException>(() => ValidationError.From(fieldErrors));

        exception.ShouldBeOfType<ArgumentException>();
        exception.ParamName.ShouldBe("fieldErrors");
    }

    [Fact]
    public void From_Null_ThrowsArgumentNullException()
    {
        var exception = Should.Throw<ArgumentNullException>(() => ValidationError.From(null!));

        exception.ParamName.ShouldBe("fieldErrors");
    }

    [Fact]
    public void Merge_OtherError_AppendsFieldErrorsInOrder()
    {
        var first = ValidationError.From([new("name", "Required"), new("email", "Invalid")]);
        var second = ValidationError.For("age", "Too young");

        var merged = first.Merge(second);

        merged.FieldErrors.ShouldBe([new FieldError("name", "Required"), new FieldError("email", "Invalid"), new FieldError("age", "Too young")]);
        merged.Message.ShouldBe("name: Required; email: Invalid; age: Too young");
        merged.Kind.ShouldBe(ErrorKind.Validation);
        merged.Code.ShouldBe("validation");
    }

    [Fact]
    public void Merge_OtherError_DoesNotChangeSources()
    {
        var first = ValidationError.For("name", "Required");
        var second = ValidationError.For("age", "Too young");

        _ = first.Merge(second);

        first.FieldErrors.ShouldBe([new FieldError("name", "Required")]);
        second.FieldErrors.ShouldBe([new FieldError("age", "Too young")]);
    }

    [Fact]
    public void Merge_Null_ThrowsArgumentNullException()
    {
        var error = ValidationError.For("name", "Required");

        var exception = Should.Throw<ArgumentNullException>(() => error.Merge(null!));

        exception.ParamName.ShouldBe("other");
    }

    [Fact]
    public void Equals_SameFieldErrors_ReturnsTrue()
    {
        var left = ValidationError.From([new("name", "Required"), new("age", "Too young")]);
        var right = ValidationError.From([new("name", "Required"), new("age", "Too young")]);

        left.Equals(right).ShouldBeTrue();
        (left == right).ShouldBeTrue();
        (left != right).ShouldBeFalse();
        left.GetHashCode().ShouldBe(right.GetHashCode());
    }

    [Fact]
    public void Equals_SameFieldErrorsAsError_ReturnsTrue()
    {
        Error left = ValidationError.For("name", "Required");
        Error right = ValidationError.For("name", "Required");

        left.Equals(right).ShouldBeTrue();
        left.Equals((Object)right).ShouldBeTrue();
        (left == right).ShouldBeTrue();
        left.GetHashCode().ShouldBe(right.GetHashCode());
    }

    [Fact]
    public void Equals_DifferentFieldErrors_ReturnsFalse()
    {
        var left = ValidationError.From([new("name", "Required"), new("age", "Too young")]);
        var right = ValidationError.From([new("age", "Too young"), new("name", "Required")]);

        left.Equals(right).ShouldBeFalse();
        (left == right).ShouldBeFalse();
        (left != right).ShouldBeTrue();
    }

    [Fact]
    public void Equals_SameMessageDifferentFieldErrors_ReturnsFalse()
    {
        var left = ValidationError.For("name", "Required");
        var right = ValidationError.For("", "name: Required");

        left.Message.ShouldBe(right.Message);
        left.Equals(right).ShouldBeFalse();
        (left == right).ShouldBeFalse();
    }

    [Fact]
    public void Equals_PlainErrorWithSameFields_ReturnsFalse()
    {
        var validationError = ValidationError.For("name", "Required");
        var error = new Error(ErrorKind.Validation, "validation", "name: Required");

        validationError.Equals(error).ShouldBeFalse();
        error.Equals(validationError).ShouldBeFalse();
        (error == validationError).ShouldBeFalse();
    }

    [Fact]
    public void ToString_MultipleFieldErrors_FormatsCodeAndJoinedMessage()
    {
        var text = ValidationError.From([new("name", "Required"), new("age", "Too young")]).ToString();

        text.ShouldBe("[validation] name: Required; age: Too young");
    }

    [Fact]
    public void FieldErrors_CannotBeCastToMutableArray()
    {
        var error = ValidationError.For("name", "Required");

        (error.FieldErrors is FieldError[]).ShouldBeFalse();
    }
}
