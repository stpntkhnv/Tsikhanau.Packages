namespace Tsikhanau.Railway.Tests;

public class FieldErrorTests
{
    [Fact]
    public void Constructor_FieldAndMessage_SetsProperties()
    {
        var fieldError = new FieldError("name", "Required");

        fieldError.Field.ShouldBe("name");
        fieldError.Message.ShouldBe("Required");
    }

    [Fact]
    public void Constructor_EmptyField_IsAllowed()
    {
        var fieldError = new FieldError("", "Object is invalid");

        fieldError.Field.ShouldBe("");
        fieldError.Message.ShouldBe("Object is invalid");
    }

    [Fact]
    public void Constructor_NullField_ThrowsArgumentNullException()
    {
        var exception = Should.Throw<ArgumentNullException>(() => new FieldError(null!, "Required"));

        exception.ParamName.ShouldBe("field");
    }

    [Fact]
    public void Constructor_NullMessage_ThrowsArgumentNullException()
    {
        var exception = Should.Throw<ArgumentNullException>(() => new FieldError("name", null!));

        exception.ParamName.ShouldBe("message");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    public void Constructor_EmptyOrWhiteSpaceMessage_ThrowsArgumentException(String message)
    {
        var exception = Should.Throw<ArgumentException>(() => new FieldError("name", message));

        exception.ShouldBeOfType<ArgumentException>();
        exception.ParamName.ShouldBe("message");
    }

    [Fact]
    public void Equals_SameFieldAndMessage_ReturnsTrue()
    {
        var left = new FieldError("name", "Required");
        var right = new FieldError("name", "Required");

        left.Equals(right).ShouldBeTrue();
        (left == right).ShouldBeTrue();
        (left != right).ShouldBeFalse();
        left.GetHashCode().ShouldBe(right.GetHashCode());
    }

    [Theory]
    [InlineData("other", "Required")]
    [InlineData("name", "Other")]
    [InlineData("NAME", "Required")]
    [InlineData("", "Required")]
    public void Equals_DifferentFieldOrMessage_ReturnsFalse(String field, String message)
    {
        var left = new FieldError("name", "Required");
        var right = new FieldError(field, message);

        left.Equals(right).ShouldBeFalse();
        (left == right).ShouldBeFalse();
        (left != right).ShouldBeTrue();
    }

    [Fact]
    public void ToString_WithField_FormatsFieldAndMessage()
    {
        var text = new FieldError("name", "Required").ToString();

        text.ShouldBe("name: Required");
    }

    [Fact]
    public void ToString_EmptyField_ReturnsMessage()
    {
        var text = new FieldError("", "Object is invalid").ToString();

        text.ShouldBe("Object is invalid");
    }

    [Fact]
    public void ToString_Default_ReturnsEmptyString()
    {
        var fieldError = default(FieldError);

        fieldError.ToString().ShouldBe(String.Empty);
    }
}
