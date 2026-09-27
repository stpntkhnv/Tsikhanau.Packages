namespace Tsikhanau.Railway.Tests;

public class UnitTests
{
    [Fact]
    public void Value_Field_EqualsDefault()
    {
        var value = Unit.Value;

        value.ShouldBe(default(Unit));
    }

    [Fact]
    public void ToString_Always_ReturnsEmptyParentheses()
    {
        var text = Unit.Value.ToString();

        text.ShouldBe("()");
    }

    [Fact]
    public void Equals_AnyUnit_ReturnsTrue()
    {
        var left = Unit.Value;
        var right = default(Unit);

        left.Equals(right).ShouldBeTrue();
        (left == right).ShouldBeTrue();
        (left != right).ShouldBeFalse();
    }

    [Fact]
    public void Equals_BoxedUnit_ReturnsTrue()
    {
        Object other = default(Unit);

        Unit.Value.Equals(other).ShouldBeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData(0)]
    [InlineData("()")]
    public void Equals_NonUnitObject_ReturnsFalse(Object? other)
    {
        var result = Unit.Value.Equals(other);

        result.ShouldBeFalse();
    }

    [Fact]
    public void GetHashCode_Always_ReturnsZero()
    {
        var hashCode = Unit.Value.GetHashCode();

        hashCode.ShouldBe(0);
    }
}
