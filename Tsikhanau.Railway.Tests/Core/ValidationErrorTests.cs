namespace Tsikhanau.Railway.Tests;

public class ValidationErrorTests
{
    private const String EmptyMessagesError = "ValidationError must contain at least one not empty message";

    public static TheoryData<String[]> BlankMessageSets => new()
    {
        Array.Empty<String>(),
        new[] { "", " ", "\t" },
        new[] { null!, "" }
    };

    [Fact]
    public void From_Messages_KeepsMessagesInOrder()
    {
        var error = ValidationError.From(new[] { "first", "second", "third" });

        error.Messages.ShouldBe(new[] { "first", "second", "third" });
    }

    [Fact]
    public void From_Messages_UsesValidationCodeWithoutInnerError()
    {
        var error = ValidationError.From(new[] { "first" });

        error.Code.ShouldBe("VALIDATION");
        error.InnerError.ShouldBeNull();
    }

    [Fact]
    public void From_Messages_JoinsMessageWithSemicolonAndNewLine()
    {
        var error = ValidationError.From(new[] { "first", "second", "third" });

        error.Message.ShouldBe("first;\nsecond;\nthird");
    }

    [Fact]
    public void From_MessagesWithBlanks_FiltersBlankMessages()
    {
        var error = ValidationError.From(new[] { "first", null!, "", " ", "\t", "second" });

        error.Messages.ShouldBe(new[] { "first", "second" });
        error.Message.ShouldBe("first;\nsecond");
    }

    [Fact]
    public void From_DuplicateMessages_KeepsFirstOccurrences()
    {
        var error = ValidationError.From(new[] { "b", "a", "b", "a", "c" });

        error.Messages.ShouldBe(new[] { "b", "a", "c" });
    }

    [Fact]
    public void From_MessagesDifferingByCaseOrPadding_KeepsAll()
    {
        var error = ValidationError.From(new[] { "a", "A", " a" });

        error.Messages.ShouldBe(new[] { "a", "A", " a" });
    }

    [Theory]
    [MemberData(nameof(BlankMessageSets))]
    public void From_NoNonBlankMessages_ThrowsArgumentException(String[] messages)
    {
        var exception = Should.Throw<ArgumentException>(() => ValidationError.From(messages));

        exception.ShouldBeOfType<ArgumentException>();
        exception.Message.ShouldBe(EmptyMessagesError);
        exception.ParamName.ShouldBeNull();
    }

    [Fact]
    public void From_NullMessages_ThrowsArgumentNullException()
    {
        Should.Throw<ArgumentNullException>(() => ValidationError.From((IEnumerable<String>)null!));
    }

    [Fact]
    public void From_Errors_UsesErrorMessages()
    {
        var error = ValidationError.From(new[] { Error.Create("A", "first"), Error.Create("B", "second") });

        error.Messages.ShouldBe(new[] { "first", "second" });
        error.Message.ShouldBe("first;\nsecond");
        error.Code.ShouldBe("VALIDATION");
        error.InnerError.ShouldBeNull();
    }

    [Fact]
    public void From_ErrorsWithDuplicateMessages_KeepsFirstOccurrencesIgnoringCodes()
    {
        var error = ValidationError.From(new[]
        {
            Error.Create("A", "same"),
            Error.Create("B", "same"),
            Error.Create("C", "other")
        });

        error.Messages.ShouldBe(new[] { "same", "other" });
    }

    [Fact]
    public void From_ErrorsWithBlankMessages_FiltersBlankMessages()
    {
        var error = ValidationError.From(new[]
        {
            Error.FromException(new Exception(" ")),
            Error.Create("A", "kept"),
            Error.FromException(new Exception(""))
        });

        error.Messages.ShouldBe(new[] { "kept" });
    }

    [Fact]
    public void From_ValidationErrors_DoesNotFlattenMessages()
    {
        var error = ValidationError.From(new Error[]
        {
            ValidationError.From(new[] { "a", "b" }),
            Error.Create("C", "c")
        });

        error.Messages.ShouldBe(new[] { "a;\nb", "c" });
    }

    [Fact]
    public void From_NoErrors_ThrowsArgumentException()
    {
        var exception = Should.Throw<ArgumentException>(() => ValidationError.From(Array.Empty<Error>()));

        exception.ShouldBeOfType<ArgumentException>();
        exception.Message.ShouldBe(EmptyMessagesError);
    }

    [Fact]
    public void From_ErrorsWithOnlyBlankMessages_ThrowsArgumentException()
    {
        var errors = new[] { Error.FromException(new Exception(" ")), Error.FromException(new Exception("")) };

        var exception = Should.Throw<ArgumentException>(() => ValidationError.From(errors));

        exception.ShouldBeOfType<ArgumentException>();
        exception.Message.ShouldBe(EmptyMessagesError);
    }

    [Fact]
    public void From_ErrorsWithNullElement_ThrowsNullReferenceException()
    {
        var errors = new[] { Error.Create("A", "first"), null! };

        Should.Throw<NullReferenceException>(() => ValidationError.From(errors));
    }

    [Fact]
    public void From_NullErrors_ThrowsArgumentNullException()
    {
        Should.Throw<ArgumentNullException>(() => ValidationError.From((IEnumerable<Error>)null!));
    }

    [Fact]
    public void Single_Message_ReturnsErrorWithOneMessage()
    {
        var error = ValidationError.Single("invalid");

        error.Messages.ShouldBe(new[] { "invalid" });
        error.Message.ShouldBe("invalid");
        error.Code.ShouldBe("VALIDATION");
        error.InnerError.ShouldBeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Single_BlankMessage_DoesNotThrow(String message)
    {
        var error = ValidationError.Single(message);

        error.Messages.ShouldBe(new[] { message });
        error.Message.ShouldBe(message);
    }

    [Fact]
    public void Single_Null_KeepsNullMessage()
    {
        var error = ValidationError.Single(null!);

        error.Messages.Count.ShouldBe(1);
        error.Messages[0].ShouldBeNull();
        error.Message.ShouldBe("");
    }

    [Fact]
    public void Equals_ErrorWithSameCodeAndMessage_ReturnsTrue()
    {
        var validationError = ValidationError.From(new[] { "a", "b" });
        var error = Error.Create("VALIDATION", "a;\nb");

        validationError.Equals(error).ShouldBeTrue();
        error.Equals(validationError).ShouldBeTrue();
        (ValidationError.Single("invalid") == Error.Validation("invalid")).ShouldBeTrue();
    }

    [Fact]
    public void ToString_MultipleMessages_FormatsCodeAndJoinedMessage()
    {
        var text = ValidationError.From(new[] { "a", "b" }).ToString();

        text.ShouldBe("[VALIDATION] a;\nb");
    }
}
