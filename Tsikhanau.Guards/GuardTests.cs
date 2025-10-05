using System.Text.RegularExpressions;
using Xunit;

namespace Tsikhanau.Guards.Tests;

public class GuardTests
{
    #region Null Guards Tests

    [Fact]
    public void AgainstNull_WithValidValue_ReturnsValue()
    {
        // Arrange
        var value = "test";

        // Act
        var result = Guard.AgainstNull(value);

        // Assert
        Assert.Equal(value, result);
    }

    [Fact]
    public void AgainstNull_WithNullValue_ThrowsArgumentNullException()
    {
        // Arrange
        string? value = null;

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => Guard.AgainstNull(value));
        Assert.Contains("value", exception.ParamName);
    }

    [Fact]
    public void AgainstNull_WithCustomMessage_ThrowsWithCustomMessage()
    {
        // Arrange
        string? value = null;
        var customMessage = "Custom error message";

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => Guard.AgainstNull(value, customMessage));
        Assert.Equal(customMessage, exception.Message.Split('\r', '\n')[0]);
    }

    #endregion

    #region String Guards Tests

    [Fact]
    public void AgainstNullOrEmpty_WithValidString_ReturnsString()
    {
        // Arrange
        var value = "test";

        // Act
        var result = Guard.AgainstNullOrEmpty(value);

        // Assert
        Assert.Equal(value, result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void AgainstNullOrEmpty_WithInvalidString_ThrowsArgumentException(string? value)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => Guard.AgainstNullOrEmpty(value));
    }

    [Fact]
    public void AgainstNullOrWhiteSpace_WithValidString_ReturnsString()
    {
        // Arrange
        var value = "test";

        // Act
        var result = Guard.AgainstNullOrWhiteSpace(value);

        // Assert
        Assert.Equal(value, result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void AgainstNullOrWhiteSpace_WithInvalidString_ThrowsArgumentException(string? value)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => Guard.AgainstNullOrWhiteSpace(value));
    }

    [Fact]
    public void AgainstTooLong_WithValidLength_ReturnsString()
    {
        // Arrange
        var value = "test";
        var maxLength = 10;

        // Act
        var result = Guard.AgainstTooLong(value, maxLength);

        // Assert
        Assert.Equal(value, result);
    }

    [Fact]
    public void AgainstTooLong_WithExcessiveLength_ThrowsArgumentException()
    {
        // Arrange
        var value = "this is a very long string";
        var maxLength = 5;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => Guard.AgainstTooLong(value, maxLength));
        Assert.Contains($"cannot exceed {maxLength} characters", exception.Message);
    }

    [Fact]
    public void AgainstTooShort_WithValidLength_ReturnsString()
    {
        // Arrange
        var value = "test string";
        var minLength = 5;

        // Act
        var result = Guard.AgainstTooShort(value, minLength);

        // Assert
        Assert.Equal(value, result);
    }

    [Fact]
    public void AgainstTooShort_WithInsufficientLength_ThrowsArgumentException()
    {
        // Arrange
        var value = "hi";
        var minLength = 5;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => Guard.AgainstTooShort(value, minLength));
        Assert.Contains($"must be at least {minLength} characters", exception.Message);
    }

    [Fact]
    public void AgainstInvalidFormat_WithValidPattern_ReturnsString()
    {
        // Arrange
        var value = "test@example.com";
        var pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$"; // Simple email pattern

        // Act
        var result = Guard.AgainstInvalidFormat(value, pattern);

        // Assert
        Assert.Equal(value, result);
    }

    [Fact]
    public void AgainstInvalidFormat_WithInvalidPattern_ThrowsArgumentException()
    {
        // Arrange
        var value = "not-an-email";
        var pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$"; // Simple email pattern

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => Guard.AgainstInvalidFormat(value, pattern));
        Assert.Contains("does not match the required format", exception.Message);
    }

    [Fact]
    public void AgainstInvalidFormat_WithCompiledRegex_ReturnsString()
    {
        // Arrange
        var value = "test@example.com";
        var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        // Act
        var result = Guard.AgainstInvalidFormat(value, regex);

        // Assert
        Assert.Equal(value, result);
    }

    #endregion

    #region Collection Guards Tests

    [Fact]
    public void AgainstNullOrEmpty_WithValidCollection_ReturnsCollection()
    {
        // Arrange
        var collection = new List<int> { 1, 2, 3 };

        // Act
        var result = Guard.AgainstNullOrEmpty(collection);

        // Assert
        Assert.Equal(collection, result);
    }

    [Fact]
    public void AgainstNullOrEmpty_WithNullCollection_ThrowsArgumentNullException()
    {
        // Arrange
        List<int>? collection = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => Guard.AgainstNullOrEmpty(collection));
    }

    [Fact]
    public void AgainstNullOrEmpty_WithEmptyCollection_ThrowsArgumentException()
    {
        // Arrange
        var collection = new List<int>();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => Guard.AgainstNullOrEmpty(collection));
    }

    [Fact]
    public void AgainstTooManyItems_WithValidCount_ReturnsCollection()
    {
        // Arrange
        var collection = new List<int> { 1, 2, 3 };
        var maxCount = 5;

        // Act
        var result = Guard.AgainstTooManyItems(collection, maxCount);

        // Assert
        Assert.Equal(collection, result);
    }

    [Fact]
    public void AgainstTooManyItems_WithExcessiveCount_ThrowsArgumentException()
    {
        // Arrange
        var collection = new List<int> { 1, 2, 3, 4, 5, 6 };
        var maxCount = 3;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => Guard.AgainstTooManyItems(collection, maxCount));
        Assert.Contains($"cannot contain more than {maxCount} items", exception.Message);
    }

    #endregion

    #region Numeric Guards Tests

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(100)]
    public void AgainstNegative_WithValidValue_ReturnsValue(int value)
    {
        // Act
        var result = Guard.AgainstNegative(value);

        // Assert
        Assert.Equal(value, result);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public void AgainstNegative_WithNegativeValue_ThrowsArgumentOutOfRangeException(int value)
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => Guard.AgainstNegative(value));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    public void AgainstNegativeOrZero_WithPositiveValue_ReturnsValue(int value)
    {
        // Act
        var result = Guard.AgainstNegativeOrZero(value);

        // Assert
        Assert.Equal(value, result);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void AgainstNegativeOrZero_WithInvalidValue_ThrowsArgumentOutOfRangeException(int value)
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => Guard.AgainstNegativeOrZero(value));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(-1)]
    [InlineData(100)]
    public void AgainstZero_WithNonZeroValue_ReturnsValue(int value)
    {
        // Act
        var result = Guard.AgainstZero(value);

        // Assert
        Assert.Equal(value, result);
    }

    [Fact]
    public void AgainstZero_WithZeroValue_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var value = 0;

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => Guard.AgainstZero(value));
    }

    [Fact]
    public void AgainstOutOfRange_WithValidValue_ReturnsValue()
    {
        // Arrange
        var value = 5;
        var min = 1;
        var max = 10;

        // Act
        var result = Guard.AgainstOutOfRange(value, min, max);

        // Assert
        Assert.Equal(value, result);
    }

    [Theory]
    [InlineData(0, 1, 10)]  // Below range
    [InlineData(11, 1, 10)] // Above range
    public void AgainstOutOfRange_WithInvalidValue_ThrowsArgumentOutOfRangeException(int value, int min, int max)
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => Guard.AgainstOutOfRange(value, min, max));
    }

    #endregion

    #region Enum Guards Tests

    public enum TestEnum
    {
        Value1 = 1,
        Value2 = 2,
        Value3 = 3
    }

    [Theory]
    [InlineData(TestEnum.Value1)]
    [InlineData(TestEnum.Value2)]
    [InlineData(TestEnum.Value3)]
    public void AgainstUndefinedEnum_WithValidEnum_ReturnsEnum(TestEnum value)
    {
        // Act
        var result = Guard.AgainstUndefinedEnum(value);

        // Assert
        Assert.Equal(value, result);
    }

    [Fact]
    public void AgainstUndefinedEnum_WithUndefinedEnum_ThrowsArgumentException()
    {
        // Arrange
        var value = (TestEnum)999;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => Guard.AgainstUndefinedEnum(value));
    }

    #endregion

    #region Type Guards Tests

    [Fact]
    public void AgainstInvalidType_WithValidType_ReturnsValue()
    {
        // Arrange
        object value = "test string";

        // Act
        var result = Guard.AgainstInvalidType<string>(value);

        // Assert
        Assert.Equal("test string", result);
    }

    [Fact]
    public void AgainstInvalidType_WithInvalidType_ThrowsArgumentException()
    {
        // Arrange
        object value = 123;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => Guard.AgainstInvalidType<string>(value));
    }

    #endregion

    #region Date/Time Guards Tests

    [Fact]
    public void AgainstFutureDate_WithPastDate_ReturnsDate()
    {
        // Arrange
        var value = DateTime.Now.AddDays(-1);

        // Act
        var result = Guard.AgainstFutureDate(value);

        // Assert
        Assert.Equal(value, result);
    }

    [Fact]
    public void AgainstFutureDate_WithFutureDate_ThrowsArgumentException()
    {
        // Arrange
        var value = DateTime.Now.AddDays(1);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => Guard.AgainstFutureDate(value));
    }

    [Fact]
    public void AgainstPastDate_WithFutureDate_ReturnsDate()
    {
        // Arrange
        var value = DateTime.Now.AddDays(1);

        // Act
        var result = Guard.AgainstPastDate(value);

        // Assert
        Assert.Equal(value, result);
    }

    [Fact]
    public void AgainstPastDate_WithPastDate_ThrowsArgumentException()
    {
        // Arrange
        var value = DateTime.Now.AddDays(-1);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => Guard.AgainstPastDate(value));
    }

    #endregion

    #region Conditional Guards Tests

    [Fact]
    public void Against_WithFalseCondition_DoesNotThrow()
    {
        // Arrange
        var condition = false;
        var message = "Error message";

        // Act & Assert (should not throw)
        Guard.Against(condition, message);
    }

    [Fact]
    public void Against_WithTrueCondition_ThrowsArgumentException()
    {
        // Arrange
        var condition = true;
        var message = "Error message";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => Guard.Against(condition, message));
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public void Against_WithExceptionFactory_ThrowsCustomException()
    {
        // Arrange
        var condition = true;
        var customException = new InvalidOperationException("Custom exception");

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => Guard.Against(condition, () => customException));
        Assert.Equal(customException.Message, exception.Message);
    }

    [Fact]
    public void Ensure_WithTrueCondition_DoesNotThrow()
    {
        // Arrange
        var condition = true;
        var message = "Error message";

        // Act & Assert (should not throw)
        Guard.Ensure(condition, message);
    }

    [Fact]
    public void Ensure_WithFalseCondition_ThrowsArgumentException()
    {
        // Arrange
        var condition = false;
        var message = "Error message";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => Guard.Ensure(condition, message));
        Assert.Equal(message, exception.Message);
    }

    #endregion

    #region Guid Guards Tests

    [Fact]
    public void AgainstEmpty_WithValidGuid_ReturnsGuid()
    {
        // Arrange
        var value = Guid.NewGuid();

        // Act
        var result = Guard.AgainstEmpty(value);

        // Assert
        Assert.Equal(value, result);
    }

    [Fact]
    public void AgainstEmpty_WithEmptyGuid_ThrowsArgumentException()
    {
        // Arrange
        var value = Guid.Empty;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => Guard.AgainstEmpty(value));
    }

    #endregion
}
