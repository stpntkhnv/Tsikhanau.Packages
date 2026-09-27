namespace Tsikhanau.Railway.Tests;

public class ErrorTests
{
    private static readonly Error Chain =
        Error.Create("OUTER", "outer", Error.Create("MIDDLE", "middle", Error.Create("INNER", "inner")));

    public static TheoryData<String?, String?, String> InvalidCodeOrMessage => new()
    {
        { null, "message", "code" },
        { "", "message", "code" },
        { " ", "message", "code" },
        { "\t", "message", "code" },
        { "CODE", null, "message" },
        { "CODE", "", "message" },
        { "CODE", " ", "message" },
        { "CODE", "\t", "message" }
    };

    [Fact]
    public void Create_CodeAndMessage_ReturnsErrorWithoutInnerError()
    {
        var error = Error.Create("CODE", "message");

        error.Code.ShouldBe("CODE");
        error.Message.ShouldBe("message");
        error.InnerError.ShouldBeNull();
    }

    [Theory]
    [MemberData(nameof(InvalidCodeOrMessage))]
    public void Create_InvalidCodeOrMessage_ThrowsArgumentException(String? code, String? message, String paramName)
    {
        var exception = Should.Throw<ArgumentException>(() => Error.Create(code!, message!));

        exception.ShouldBeOfType<ArgumentException>();
        exception.ParamName.ShouldBe(paramName);
    }

    [Fact]
    public void Create_WithInnerError_ReturnsErrorWithInnerError()
    {
        var inner = Error.Create("INNER", "inner");

        var error = Error.Create("CODE", "message", inner);

        error.Code.ShouldBe("CODE");
        error.Message.ShouldBe("message");
        error.InnerError.ShouldBeSameAs(inner);
    }

    [Theory]
    [MemberData(nameof(InvalidCodeOrMessage))]
    public void Create_WithInnerErrorAndInvalidCodeOrMessage_ThrowsArgumentException(String? code, String? message, String paramName)
    {
        var inner = Error.Create("INNER", "inner");

        var exception = Should.Throw<ArgumentException>(() => Error.Create(code!, message!, inner));

        exception.ShouldBeOfType<ArgumentException>();
        exception.ParamName.ShouldBe(paramName);
    }

    [Fact]
    public void Create_NullInnerError_ThrowsArgumentNullException()
    {
        var exception = Should.Throw<ArgumentNullException>(() => Error.Create("CODE", "message", null!));

        exception.ParamName.ShouldBe("innerError");
    }

    [Fact]
    public void FromException_ExceptionWithoutInner_UsesTypeNameAndMessage()
    {
        var error = Error.FromException(new InvalidOperationException("broken"));

        error.Code.ShouldBe("InvalidOperationException");
        error.Message.ShouldBe("broken");
        error.InnerError.ShouldBeNull();
    }

    [Fact]
    public void FromException_NestedInnerExceptions_BecomeInnerErrors()
    {
        var exception = new InvalidOperationException("outer", new ArgumentException("middle", new TimeoutException("inner")));

        var error = Error.FromException(exception);

        error.Code.ShouldBe("InvalidOperationException");
        error.Message.ShouldBe("outer");
        var middle = error.InnerError.ShouldNotBeNull();
        middle.Code.ShouldBe("ArgumentException");
        middle.Message.ShouldBe("middle");
        var inner = middle.InnerError.ShouldNotBeNull();
        inner.Code.ShouldBe("TimeoutException");
        inner.Message.ShouldBe("inner");
        inner.InnerError.ShouldBeNull();
    }

    [Fact]
    public void FromException_WhitespaceMessage_KeepsMessageWithoutGuard()
    {
        var error = Error.FromException(new Exception(" "));

        error.Code.ShouldBe("Exception");
        error.Message.ShouldBe(" ");
    }

    [Fact]
    public void FromException_Null_ThrowsArgumentNullException()
    {
        var exception = Should.Throw<ArgumentNullException>(() => Error.FromException(null!));

        exception.ParamName.ShouldBe("exception");
    }

    [Fact]
    public void WithContext_CodeAndMessage_WrapsOriginalAsInnerError()
    {
        var original = Error.Create("INNER", "inner");

        var error = original.WithContext("OUTER", "outer");

        error.Code.ShouldBe("OUTER");
        error.Message.ShouldBe("outer");
        error.InnerError.ShouldBeSameAs(original);
    }

    [Theory]
    [MemberData(nameof(InvalidCodeOrMessage))]
    public void WithContext_InvalidCodeOrMessage_ThrowsArgumentException(String? code, String? message, String paramName)
    {
        var original = Error.Create("INNER", "inner");

        var exception = Should.Throw<ArgumentException>(() => original.WithContext(code!, message!));

        exception.ShouldBeOfType<ArgumentException>();
        exception.ParamName.ShouldBe(paramName);
    }

    [Fact]
    public void Validation_Message_ReturnsPlainErrorWithValidationCode()
    {
        var error = Error.Validation("invalid");

        error.Code.ShouldBe("VALIDATION");
        error.Message.ShouldBe("invalid");
        error.InnerError.ShouldBeNull();
        error.ShouldBeOfType<Error>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validation_InvalidMessage_ThrowsArgumentException(String? message)
    {
        var exception = Should.Throw<ArgumentException>(() => Error.Validation(message!));

        exception.ShouldBeOfType<ArgumentException>();
        exception.ParamName.ShouldBe("message");
    }

    [Fact]
    public void Unknown_Field_HasUnknownCodeAndMessage()
    {
        var error = Error.Unknown;

        error.Code.ShouldBe("UNKNOWN");
        error.Message.ShouldBe("Unexpected error");
        error.InnerError.ShouldBeNull();
    }

    [Fact]
    public void GetFullMessage_NoInnerError_ReturnsMessage()
    {
        var message = Error.Create("CODE", "message").GetFullMessage();

        message.ShouldBe("message");
    }

    [Fact]
    public void GetFullMessage_InnerErrorChain_JoinsMessagesFromOuterToInner()
    {
        var message = Chain.GetFullMessage();

        message.ShouldBe("outer -> middle -> inner");
    }

    [Fact]
    public void GetAllCodes_NoInnerError_ReturnsOwnCode()
    {
        var codes = Error.Create("CODE", "message").GetAllCodes();

        codes.ShouldBe(new[] { "CODE" });
    }

    [Fact]
    public void GetAllCodes_InnerErrorChain_ReturnsCodesFromOuterToInner()
    {
        var codes = Chain.GetAllCodes();

        codes.ShouldBe(new[] { "OUTER", "MIDDLE", "INNER" });
    }

    [Fact]
    public void Equals_SameCodeAndMessage_ReturnsTrue()
    {
        var left = Error.Create("CODE", "message");
        var right = Error.Create("CODE", "message");

        left.Equals(right).ShouldBeTrue();
        (left == right).ShouldBeTrue();
        (left != right).ShouldBeFalse();
        left.GetHashCode().ShouldBe(right.GetHashCode());
    }

    [Theory]
    [InlineData("OTHER", "message")]
    [InlineData("CODE", "other")]
    [InlineData("code", "message")]
    [InlineData("CODE", "MESSAGE")]
    public void Equals_DifferentCodeOrMessage_ReturnsFalse(String code, String message)
    {
        var left = Error.Create("CODE", "message");
        var right = Error.Create(code, message);

        left.Equals(right).ShouldBeFalse();
        (left == right).ShouldBeFalse();
        (left != right).ShouldBeTrue();
    }

    [Fact]
    public void Equals_EqualInnerErrors_ReturnsTrue()
    {
        var left = Error.Create("CODE", "message", Error.Create("INNER", "inner"));
        var right = Error.Create("CODE", "message", Error.Create("INNER", "inner"));

        left.Equals(right).ShouldBeTrue();
        (left == right).ShouldBeTrue();
        left.GetHashCode().ShouldBe(right.GetHashCode());
    }

    [Fact]
    public void Equals_DifferentInnerErrors_ReturnsFalse()
    {
        var left = Error.Create("CODE", "message", Error.Create("INNER", "inner"));
        var right = Error.Create("CODE", "message", Error.Create("INNER", "other"));

        left.Equals(right).ShouldBeFalse();
        (left == right).ShouldBeFalse();
        (left != right).ShouldBeTrue();
    }

    [Fact]
    public void Equals_InnerErrorOnOneSideOnly_ReturnsFalse()
    {
        var left = Error.Create("CODE", "message", Error.Create("INNER", "inner"));
        var right = Error.Create("CODE", "message");

        left.Equals(right).ShouldBeFalse();
        right.Equals(left).ShouldBeFalse();
        (left == right).ShouldBeFalse();
    }

    [Fact]
    public void Equals_Null_ReturnsFalse()
    {
        var error = Error.Create("CODE", "message");

        error.Equals(null).ShouldBeFalse();
        (error == null).ShouldBeFalse();
        (null != error).ShouldBeTrue();
    }

    [Fact]
    public void Equals_BoxedEqualError_ReturnsTrue()
    {
        var error = Error.Create("CODE", "message");
        Object other = Error.Create("CODE", "message");

        error.Equals(other).ShouldBeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData(5)]
    [InlineData("message")]
    public void Equals_NonErrorObject_ReturnsFalse(Object? other)
    {
        var error = Error.Create("GENERAL", "message");

        error.Equals(other).ShouldBeFalse();
    }

    [Fact]
    public void Equals_UnboxedString_ConvertsImplicitlyToGeneralError()
    {
        var error = Error.Create("GENERAL", "message");

        error.Equals("message").ShouldBeTrue();
        (error == "message").ShouldBeTrue();
    }

    [Fact]
    public void Equality_EmptyString_ThrowsArgumentException()
    {
        var error = Error.Create("GENERAL", "message");

        Should.Throw<ArgumentException>(() => error == "");
    }

    [Fact]
    public void ToString_NoInnerError_FormatsCodeAndMessage()
    {
        var text = Error.Create("CODE", "message").ToString();

        text.ShouldBe("[CODE] message");
    }

    [Fact]
    public void ToString_InnerErrorChain_AppendsInnerErrors()
    {
        var text = Chain.ToString();

        text.ShouldBe("[OUTER] outer -> [MIDDLE] middle -> [INNER] inner");
    }

    [Fact]
    public void ImplicitConversion_FromString_CreatesGeneralError()
    {
        Error error = "boom";

        error.Code.ShouldBe("GENERAL");
        error.Message.ShouldBe("boom");
        error.InnerError.ShouldBeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void ImplicitConversion_FromInvalidString_ThrowsArgumentException(String? message)
    {
        var exception = Should.Throw<ArgumentException>(() =>
        {
            Error error = message!;
            return error;
        });

        exception.ShouldBeOfType<ArgumentException>();
        exception.ParamName.ShouldBe("message");
    }
}
