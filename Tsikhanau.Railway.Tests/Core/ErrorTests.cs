namespace Tsikhanau.Railway.Tests;

public class ErrorTests
{
    private static readonly Error Chain = Error.NotFound("inner", "Inner")
        .WithContext("middle", "Middle")
        .WithContext("outer", "Outer");

    public static TheoryData<ErrorKind, Error> Factories => new()
    {
        { ErrorKind.Failure, Error.Failure("code", "message") },
        { ErrorKind.Unexpected, Error.Unexpected("code", "message") },
        { ErrorKind.NotFound, Error.NotFound("code", "message") },
        { ErrorKind.Conflict, Error.Conflict("code", "message") },
        { ErrorKind.Unauthorized, Error.Unauthorized("code", "message") },
        { ErrorKind.Forbidden, Error.Forbidden("code", "message") }
    };

    public static TheoryData<String?, String?, String> NullCodeOrMessage => new()
    {
        { null, "message", "code" },
        { "code", null, "message" }
    };

    public static TheoryData<String, String, String> EmptyCodeOrMessage => new()
    {
        { "", "message", "code" },
        { " ", "message", "code" },
        { "\t", "message", "code" },
        { "code", "", "message" },
        { "code", " ", "message" },
        { "code", "\t", "message" }
    };

    [Fact]
    public void Constructor_ValidArguments_SetsKindCodeAndMessage()
    {
        var error = new Error(ErrorKind.Conflict, "code", "message");

        error.Kind.ShouldBe(ErrorKind.Conflict);
        error.Code.ShouldBe("code");
        error.Message.ShouldBe("message");
        error.Inner.ShouldBeNull();
        error.Metadata.ShouldBeNull();
    }

    [Theory]
    [MemberData(nameof(NullCodeOrMessage))]
    public void Constructor_NullCodeOrMessage_ThrowsArgumentNullException(String? code, String? message, String paramName)
    {
        var exception = Should.Throw<ArgumentNullException>(() => new Error(ErrorKind.Failure, code!, message!));

        exception.ParamName.ShouldBe(paramName);
    }

    [Theory]
    [MemberData(nameof(EmptyCodeOrMessage))]
    public void Constructor_EmptyOrWhiteSpaceCodeOrMessage_ThrowsArgumentException(String code, String message, String paramName)
    {
        var exception = Should.Throw<ArgumentException>(() => new Error(ErrorKind.Failure, code, message));

        exception.ShouldBeOfType<ArgumentException>();
        exception.ParamName.ShouldBe(paramName);
    }

    [Theory]
    [MemberData(nameof(Factories))]
    public void Factory_CodeAndMessage_SetsKindCodeAndMessage(ErrorKind kind, Error error)
    {
        error.ShouldBeOfType<Error>();
        error.Kind.ShouldBe(kind);
        error.Code.ShouldBe("code");
        error.Message.ShouldBe("message");
        error.Inner.ShouldBeNull();
        error.Metadata.ShouldBeNull();
    }

    [Theory]
    [MemberData(nameof(EmptyCodeOrMessage))]
    public void Factory_EmptyOrWhiteSpaceCodeOrMessage_ThrowsArgumentException(String code, String message, String paramName)
    {
        var exception = Should.Throw<ArgumentException>(() => Error.NotFound(code, message));

        exception.ShouldBeOfType<ArgumentException>();
        exception.ParamName.ShouldBe(paramName);
    }

    [Fact]
    public void FromException_ExceptionWithoutInner_UsesUnexpectedKindTypeNameAndMessage()
    {
        var error = Error.FromException(new InvalidOperationException("broken"));

        error.Kind.ShouldBe(ErrorKind.Unexpected);
        error.Code.ShouldBe("InvalidOperationException");
        error.Message.ShouldBe("broken");
        error.Inner.ShouldBeNull();
    }

    [Fact]
    public void FromException_NestedInnerExceptions_BecomeInnerErrors()
    {
        var exception = new InvalidOperationException("outer", new ArgumentException("middle", new TimeoutException("inner")));

        var error = Error.FromException(exception);

        error.Code.ShouldBe("InvalidOperationException");
        error.Message.ShouldBe("outer");
        var middle = error.Inner.ShouldNotBeNull();
        middle.Kind.ShouldBe(ErrorKind.Unexpected);
        middle.Code.ShouldBe("ArgumentException");
        middle.Message.ShouldBe("middle");
        var inner = middle.Inner.ShouldNotBeNull();
        inner.Kind.ShouldBe(ErrorKind.Unexpected);
        inner.Code.ShouldBe("TimeoutException");
        inner.Message.ShouldBe("inner");
        inner.Inner.ShouldBeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void FromException_EmptyOrWhiteSpaceMessage_UsesTypeNameAsMessage(String message)
    {
        var error = Error.FromException(new InvalidOperationException(message));

        error.Code.ShouldBe("InvalidOperationException");
        error.Message.ShouldBe("InvalidOperationException");
    }

    [Fact]
    public void FromException_Null_ThrowsArgumentNullException()
    {
        var exception = Should.Throw<ArgumentNullException>(() => Error.FromException(null!));

        exception.ParamName.ShouldBe("exception");
    }

    [Fact]
    public void WithContext_CodeAndMessage_KeepsKindAndWrapsOriginalAsInner()
    {
        var original = Error.NotFound("inner", "Inner");

        var error = original.WithContext("outer", "Outer");

        error.Kind.ShouldBe(ErrorKind.NotFound);
        error.Code.ShouldBe("outer");
        error.Message.ShouldBe("Outer");
        error.Inner.ShouldBeSameAs(original);
    }

    [Fact]
    public void WithContext_ValidationError_ReturnsPlainErrorWithValidationKind()
    {
        var original = Error.Validation("name", "Required");

        var error = original.WithContext("outer", "Outer");

        error.ShouldBeOfType<Error>();
        error.Kind.ShouldBe(ErrorKind.Validation);
        error.Inner.ShouldBeSameAs(original);
    }

    [Theory]
    [MemberData(nameof(EmptyCodeOrMessage))]
    public void WithContext_EmptyOrWhiteSpaceCodeOrMessage_ThrowsArgumentException(String code, String message, String paramName)
    {
        var original = Error.Failure("inner", "Inner");

        var exception = Should.Throw<ArgumentException>(() => original.WithContext(code, message));

        exception.ShouldBeOfType<ArgumentException>();
        exception.ParamName.ShouldBe(paramName);
    }

    [Theory]
    [MemberData(nameof(NullCodeOrMessage))]
    public void WithContext_NullCodeOrMessage_ThrowsArgumentNullException(String? code, String? message, String paramName)
    {
        var original = Error.Failure("inner", "Inner");

        var exception = Should.Throw<ArgumentNullException>(() => original.WithContext(code!, message!));

        exception.ParamName.ShouldBe(paramName);
    }

    [Fact]
    public void WithMetadata_NoMetadata_AddsEntryAndKeepsOtherProperties()
    {
        var inner = Error.Failure("inner", "Inner");
        var original = Error.Conflict("code", "message") with { Inner = inner };

        var error = original.WithMetadata("id", 42);

        var metadata = error.Metadata.ShouldNotBeNull();
        metadata.Count.ShouldBe(1);
        metadata["id"].ShouldBe(42);
        error.Kind.ShouldBe(ErrorKind.Conflict);
        error.Code.ShouldBe("code");
        error.Message.ShouldBe("message");
        error.Inner.ShouldBeSameAs(inner);
    }

    [Fact]
    public void WithMetadata_ExistingMetadata_KeepsExistingEntries()
    {
        var error = Error.Failure("code", "message").WithMetadata("id", 42).WithMetadata("name", "value");

        var metadata = error.Metadata.ShouldNotBeNull();
        metadata.Count.ShouldBe(2);
        metadata["id"].ShouldBe(42);
        metadata["name"].ShouldBe("value");
    }

    [Fact]
    public void WithMetadata_ExistingKey_ReplacesValue()
    {
        var error = Error.Failure("code", "message").WithMetadata("id", 1).WithMetadata("id", 2);

        var metadata = error.Metadata.ShouldNotBeNull();
        metadata.Count.ShouldBe(1);
        metadata["id"].ShouldBe(2);
    }

    [Fact]
    public void WithMetadata_NullValue_AddsEntryWithNullValue()
    {
        var error = Error.Failure("code", "message").WithMetadata("id", null);

        var metadata = error.Metadata.ShouldNotBeNull();
        metadata.ContainsKey("id").ShouldBeTrue();
        metadata["id"].ShouldBeNull();
    }

    [Fact]
    public void WithMetadata_Original_IsNotMutated()
    {
        var original = Error.Failure("code", "message");
        var withOne = original.WithMetadata("id", 1);

        var withTwo = withOne.WithMetadata("name", "value");

        original.Metadata.ShouldBeNull();
        withOne.Metadata.ShouldNotBeNull().Count.ShouldBe(1);
        withTwo.Metadata.ShouldNotBeNull().Count.ShouldBe(2);
    }

    [Fact]
    public void WithMetadata_ValidationError_KeepsRuntimeType()
    {
        var original = Error.Validation("name", "Required");

        var error = original.WithMetadata("id", 42);

        var validationError = error.ShouldBeOfType<ValidationError>();
        validationError.FieldErrors.ShouldBe(original.FieldErrors);
        validationError.Metadata.ShouldNotBeNull()["id"].ShouldBe(42);
    }

    [Fact]
    public void WithMetadata_CustomError_KeepsRuntimeTypeAndFields()
    {
        var error = new UserNotFoundError(7).WithMetadata("id", 42);

        var userNotFound = error.ShouldBeOfType<UserNotFoundError>();
        userNotFound.UserId.ShouldBe(7);
        userNotFound.Metadata.ShouldNotBeNull()["id"].ShouldBe(42);
    }

    [Fact]
    public void WithMetadata_NullKey_ThrowsArgumentNullException()
    {
        var error = Error.Failure("code", "message");

        var exception = Should.Throw<ArgumentNullException>(() => error.WithMetadata(null!, 42));

        exception.ParamName.ShouldBe("key");
    }

    [Fact]
    public void GetFullMessage_NoInner_ReturnsMessage()
    {
        var message = Error.Failure("code", "message").GetFullMessage();

        message.ShouldBe("message");
    }

    [Fact]
    public void GetFullMessage_InnerChain_JoinsMessagesFromOuterToInner()
    {
        var message = Chain.GetFullMessage();

        message.ShouldBe("Outer -> Middle -> Inner");
    }

    [Fact]
    public void GetAllCodes_NoInner_ReturnsOwnCode()
    {
        var codes = Error.Failure("code", "message").GetAllCodes();

        codes.ShouldBe(["code"]);
    }

    [Fact]
    public void GetAllCodes_InnerChain_ReturnsCodesFromOuterToInner()
    {
        var codes = Chain.GetAllCodes();

        codes.ShouldBe(["outer", "middle", "inner"]);
    }

    [Fact]
    public void ToString_NoInner_FormatsCodeAndMessage()
    {
        var text = Error.Failure("code", "message").ToString();

        text.ShouldBe("[code] message");
    }

    [Fact]
    public void ToString_InnerChain_AppendsInnerErrors()
    {
        var text = Chain.ToString();

        text.ShouldBe("[outer] Outer -> [middle] Middle -> [inner] Inner");
    }

    [Fact]
    public void ToString_CustomError_UsesErrorFormat()
    {
        var text = new UserNotFoundError(7).ToString();

        text.ShouldBe("[user.not_found] User not found");
    }

    [Fact]
    public void Equals_SameKindCodeAndMessage_ReturnsTrue()
    {
        var left = Error.Failure("code", "message");
        var right = Error.Failure("code", "message");

        left.Equals(right).ShouldBeTrue();
        (left == right).ShouldBeTrue();
        (left != right).ShouldBeFalse();
        left.GetHashCode().ShouldBe(right.GetHashCode());
    }

    [Theory]
    [InlineData(ErrorKind.Conflict, "code", "message")]
    [InlineData(ErrorKind.Failure, "other", "message")]
    [InlineData(ErrorKind.Failure, "code", "other")]
    [InlineData(ErrorKind.Failure, "CODE", "message")]
    [InlineData(ErrorKind.Failure, "code", "MESSAGE")]
    public void Equals_DifferentKindCodeOrMessage_ReturnsFalse(ErrorKind kind, String code, String message)
    {
        var left = Error.Failure("code", "message");
        var right = new Error(kind, code, message);

        left.Equals(right).ShouldBeFalse();
        (left == right).ShouldBeFalse();
        (left != right).ShouldBeTrue();
    }

    [Fact]
    public void Equals_EqualInner_ReturnsTrue()
    {
        var left = Error.Failure("inner", "Inner").WithContext("outer", "Outer");
        var right = Error.Failure("inner", "Inner").WithContext("outer", "Outer");

        left.Equals(right).ShouldBeTrue();
        (left == right).ShouldBeTrue();
        left.GetHashCode().ShouldBe(right.GetHashCode());
    }

    [Fact]
    public void Equals_DifferentInner_ReturnsFalse()
    {
        var left = Error.Failure("inner", "Inner").WithContext("outer", "Outer");
        var right = Error.Failure("inner", "Other").WithContext("outer", "Outer");

        left.Equals(right).ShouldBeFalse();
        (left == right).ShouldBeFalse();
        (left != right).ShouldBeTrue();
    }

    [Fact]
    public void Equals_InnerOnOneSideOnly_ReturnsFalse()
    {
        var left = Error.Failure("inner", "Inner").WithContext("outer", "Outer");
        var right = Error.Failure("outer", "Outer");

        left.Equals(right).ShouldBeFalse();
        right.Equals(left).ShouldBeFalse();
        (left == right).ShouldBeFalse();
    }

    [Fact]
    public void Equals_Null_ReturnsFalse()
    {
        var error = Error.Failure("code", "message");

        error.Equals(null).ShouldBeFalse();
        (error == null).ShouldBeFalse();
        (null != error).ShouldBeTrue();
    }

    [Fact]
    public void Equals_BoxedEqualError_ReturnsTrue()
    {
        var error = Error.Failure("code", "message");
        Object other = Error.Failure("code", "message");

        error.Equals(other).ShouldBeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData(5)]
    [InlineData("message")]
    public void Equals_NonErrorObject_ReturnsFalse(Object? other)
    {
        var error = Error.Failure("code", "message");

        error.Equals(other).ShouldBeFalse();
    }

    [Fact]
    public void Equals_ErrorAndSubclassWithSameFields_ReturnsFalse()
    {
        var error = new Error(ErrorKind.Unexpected, "timeout", "Timed out");
        var subclass = new TimeoutError();

        error.Equals(subclass).ShouldBeFalse();
        subclass.Equals(error).ShouldBeFalse();
        (error == subclass).ShouldBeFalse();
        (subclass == error).ShouldBeFalse();
    }

    [Fact]
    public void Equals_CustomErrorsWithSameFields_ReturnsTrue()
    {
        Error left = new UserNotFoundError(7);
        Error right = new UserNotFoundError(7);

        left.Equals(right).ShouldBeTrue();
        (left == right).ShouldBeTrue();
        left.GetHashCode().ShouldBe(right.GetHashCode());
    }

    [Fact]
    public void Equals_CustomErrorsWithDifferentFields_ReturnsFalse()
    {
        Error left = new UserNotFoundError(7);
        Error right = new UserNotFoundError(8);

        left.Equals(right).ShouldBeFalse();
        (left == right).ShouldBeFalse();
    }

    [Fact]
    public void CustomError_Constructor_SetsBaseKindCodeAndMessage()
    {
        var error = new UserNotFoundError(7);

        error.Kind.ShouldBe(ErrorKind.NotFound);
        error.Code.ShouldBe("user.not_found");
        error.Message.ShouldBe("User not found");
        error.UserId.ShouldBe(7);
    }

    [Fact]
    public void CustomError_ImplicitConversionToResult_MatchesByPattern()
    {
        Result<String> result = new UserNotFoundError(7);

        var text = result.Error switch
        {
            UserNotFoundError { UserId: var userId } => $"user {userId}",
            _ => "other"
        };

        result.IsFailure.ShouldBeTrue();
        text.ShouldBe("user 7");
    }

    private sealed record UserNotFoundError(Int32 UserId) : Error(ErrorKind.NotFound, "user.not_found", "User not found");

    private sealed record TimeoutError() : Error(ErrorKind.Unexpected, "timeout", "Timed out");

    [Fact]
    public void Equals_SameMetadataContent_ReturnsTrue()
    {
        var first = Error.Failure("code", "message").WithMetadata("id", 42);
        var second = Error.Failure("code", "message").WithMetadata("id", 42);

        first.ShouldBe(second);
        first.GetHashCode().ShouldBe(second.GetHashCode());
    }

    [Fact]
    public void Equals_DifferentMetadataValue_ReturnsFalse()
    {
        var first = Error.Failure("code", "message").WithMetadata("id", 42);
        var second = Error.Failure("code", "message").WithMetadata("id", 43);

        first.ShouldNotBe(second);
    }

    [Fact]
    public void Equals_EmptyMetadataAndNoMetadata_ReturnsTrue()
    {
        var withEmpty = Error.Failure("code", "message") with { Metadata = new Dictionary<String, Object?>() };
        var withoutMetadata = Error.Failure("code", "message");

        withEmpty.ShouldBe(withoutMetadata);
        withEmpty.GetHashCode().ShouldBe(withoutMetadata.GetHashCode());
    }

    [Fact]
    public void With_Inner_KeepsKindCodeAndMessage()
    {
        var inner = Error.Failure("inner", "Inner failed");
        var error = Error.NotFound("code", "message");

        var copy = error with { Inner = inner };

        copy.Kind.ShouldBe(ErrorKind.NotFound);
        copy.Code.ShouldBe("code");
        copy.Message.ShouldBe("message");
        copy.Inner.ShouldBe(inner);
    }

    [Fact]
    public void Equals_DifferentMetadataCount_ReturnsFalse()
    {
        var first = Error.Failure("code", "message").WithMetadata("id", 42);
        var second = first.WithMetadata("name", "x");

        first.ShouldNotBe(second);
    }
}
