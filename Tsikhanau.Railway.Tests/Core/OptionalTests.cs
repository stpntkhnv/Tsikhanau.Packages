namespace Tsikhanau.Railway.Tests;

public class OptionalTests
{
    [Fact]
    public void Some_Value_ReturnsOptionalWithValue()
    {
        var optional = Optional<Int32>.Some(5);

        optional.HasValue.ShouldBeTrue();
        optional.IsNone.ShouldBeFalse();
        optional.Value.ShouldBe(5);
    }

    [Fact]
    public void Some_Null_ThrowsArgumentNullException()
    {
        var exception = Should.Throw<ArgumentNullException>(() => Optional<String>.Some(null!));

        exception.ParamName.ShouldBe("value");
    }

    [Fact]
    public void None_NoArguments_ReturnsEmptyOptional()
    {
        var optional = Optional<Int32>.None();

        optional.HasValue.ShouldBeFalse();
        optional.IsNone.ShouldBeTrue();
    }

    [Fact]
    public void Value_None_ThrowsInvalidOperationException()
    {
        var optional = Optional<Int32>.None();

        var exception = Should.Throw<InvalidOperationException>(() => optional.Value);

        exception.Message.ShouldBe("Cannot access Value on an empty optional.");
    }

    [Fact]
    public void FromNullable_Value_ReturnsSome()
    {
        var optional = Optional<String>.FromNullable("text");

        optional.HasValue.ShouldBeTrue();
        optional.Value.ShouldBe("text");
    }

    [Fact]
    public void FromNullable_Null_ReturnsNone()
    {
        var optional = Optional<String>.FromNullable(null);

        optional.IsNone.ShouldBeTrue();
    }

    [Fact]
    public void FromNullable_ValueTypeDefault_ReturnsSome()
    {
        var optional = Optional<Int32>.FromNullable(0);

        optional.HasValue.ShouldBeTrue();
        optional.Value.ShouldBe(0);
    }

    [Fact]
    public void GetValueOrDefault_SomeWithDefaultValue_ReturnsValue()
    {
        var value = Optional<Int32>.Some(5).GetValueOrDefault(10);

        value.ShouldBe(5);
    }

    [Fact]
    public void GetValueOrDefault_NoneWithDefaultValue_ReturnsDefaultValue()
    {
        var value = Optional<Int32>.None().GetValueOrDefault(10);

        value.ShouldBe(10);
    }

    [Fact]
    public void GetValueOrDefault_NoneWithoutArguments_ReturnsTypeDefault()
    {
        var number = Optional<Int32>.None().GetValueOrDefault();
        var text = Optional<String>.None().GetValueOrDefault();

        number.ShouldBe(0);
        text.ShouldBeNull();
    }

    [Fact]
    public void GetValueOrDefault_SomeWithFactory_ReturnsValueWithoutCallingFactory()
    {
        var factory = Substitute.For<Func<Int32>>();

        var value = Optional<Int32>.Some(5).GetValueOrDefault(factory);

        value.ShouldBe(5);
        factory.DidNotReceive().Invoke();
    }

    [Fact]
    public void GetValueOrDefault_NoneWithFactory_ReturnsFactoryValue()
    {
        var factory = Substitute.For<Func<Int32>>();
        factory.Invoke().Returns(10);

        var value = Optional<Int32>.None().GetValueOrDefault(factory);

        value.ShouldBe(10);
        factory.Received(1).Invoke();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GetValueOrDefault_NullFactory_ThrowsArgumentNullException(Boolean hasValue)
    {
        var optional = hasValue ? Optional<Int32>.Some(5) : Optional<Int32>.None();
        Func<Int32> factory = null!;

        var exception = Should.Throw<ArgumentNullException>(() => optional.GetValueOrDefault(factory));

        exception.ParamName.ShouldBe("defaultValueFactory");
    }

    [Fact]
    public void ToNullable_Some_ReturnsValue()
    {
        var value = Optional<String>.Some("text").ToNullable();

        value.ShouldBe("text");
    }

    [Fact]
    public void ToNullable_ReferenceTypeNone_ReturnsNull()
    {
        var value = Optional<String>.None().ToNullable();

        value.ShouldBeNull();
    }

    [Fact]
    public void ToNullable_ValueTypeNone_ReturnsTypeDefault()
    {
        var value = Optional<Int32>.None().ToNullable();

        value.ShouldBe(0);
    }

    [Fact]
    public void Equals_SomesWithEqualValues_ReturnsTrue()
    {
        var left = Optional<String>.Some("text");
        var right = Optional<String>.Some("text");

        left.Equals(right).ShouldBeTrue();
        (left == right).ShouldBeTrue();
        (left != right).ShouldBeFalse();
        left.GetHashCode().ShouldBe(right.GetHashCode());
    }

    [Fact]
    public void Equals_SomesWithDifferentValues_ReturnsFalse()
    {
        var left = Optional<Int32>.Some(5);
        var right = Optional<Int32>.Some(6);

        left.Equals(right).ShouldBeFalse();
        (left == right).ShouldBeFalse();
        (left != right).ShouldBeTrue();
    }

    [Fact]
    public void Equals_Nones_ReturnsTrue()
    {
        var left = Optional<Int32>.None();
        var right = Optional<Int32>.None();

        left.Equals(right).ShouldBeTrue();
        (left == right).ShouldBeTrue();
        (left != right).ShouldBeFalse();
        left.GetHashCode().ShouldBe(right.GetHashCode());
    }

    [Fact]
    public void Equals_SomeAndNone_ReturnsFalse()
    {
        var some = Optional<Int32>.Some(0);
        var none = Optional<Int32>.None();

        some.Equals(none).ShouldBeFalse();
        (some == none).ShouldBeFalse();
        (some != none).ShouldBeTrue();
    }

    [Fact]
    public void Equals_BoxedEqualOptional_ReturnsTrue()
    {
        var optional = Optional<Int32>.Some(5);
        Object other = Optional<Int32>.Some(5);

        optional.Equals(other).ShouldBeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData(5)]
    [InlineData("5")]
    public void Equals_NonOptionalObject_ReturnsFalse(Object? other)
    {
        var optional = Optional<Int32>.Some(5);

        optional.Equals(other).ShouldBeFalse();
    }

    [Fact]
    public void Equals_UnboxedValue_ConvertsImplicitlyAndReturnsTrue()
    {
        var optional = Optional<Int32>.Some(5);

        optional.Equals(5).ShouldBeTrue();
        (optional == 5).ShouldBeTrue();
    }

    [Fact]
    public void GetHashCode_Some_ReturnsValueHashCode()
    {
        var hashCode = Optional<String>.Some("text").GetHashCode();

        hashCode.ShouldBe("text".GetHashCode());
    }

    [Fact]
    public void GetHashCode_None_ReturnsZero()
    {
        var hashCode = Optional<String>.None().GetHashCode();

        hashCode.ShouldBe(0);
    }

    [Fact]
    public void ToString_Some_FormatsValue()
    {
        var text = Optional<Int32>.Some(5).ToString();

        text.ShouldBe("Some(5)");
    }

    [Fact]
    public void ToString_None_ReturnsNone()
    {
        var text = Optional<Int32>.None().ToString();

        text.ShouldBe("None");
    }

    [Fact]
    public void ImplicitConversion_FromValue_ReturnsSome()
    {
        Optional<String> optional = "text";

        optional.HasValue.ShouldBeTrue();
        optional.Value.ShouldBe("text");
    }

    [Fact]
    public void ImplicitConversion_FromNull_ReturnsNone()
    {
        String? value = null;

        Optional<String> optional = value;

        optional.IsNone.ShouldBeTrue();
    }

    [Fact]
    public void ImplicitConversion_SomeToValue_ReturnsValue()
    {
        String? value = Optional<String>.Some("text");

        value.ShouldBe("text");
    }

    [Fact]
    public void ImplicitConversion_NoneToReferenceType_ReturnsNull()
    {
        String? value = Optional<String>.None();

        value.ShouldBeNull();
    }

    [Fact]
    public void ImplicitConversion_NoneToValueType_ReturnsTypeDefault()
    {
        Int32 value = Optional<Int32>.None();

        value.ShouldBe(0);
    }

    [Fact]
    public void Default_Optional_IsNone()
    {
        var optional = default(Optional<Int32>);

        optional.IsNone.ShouldBeTrue();
        optional.HasValue.ShouldBeFalse();
        optional.ShouldBe(Optional<Int32>.None());
    }
}
