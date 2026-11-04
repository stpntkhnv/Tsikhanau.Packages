namespace Tsikhanau.Railway.Tests;

public class NullableConversionsTests
{
    [Fact]
    public void AsOptional_StructWithValue_ReturnsSome()
    {
        Int32? value = 5;

        var optional = value.AsOptional();

        optional.ShouldBe(Optional<Int32>.Some(5));
    }

    [Fact]
    public void AsOptional_StructWithoutValue_ReturnsNone()
    {
        Int32? value = null;

        var optional = value.AsOptional();

        optional.ShouldBe(Optional<Int32>.None());
    }

    [Fact]
    public void AsOptional_ClassWithValue_ReturnsSome()
    {
        String? value = "a";

        var optional = value.AsOptional();

        optional.ShouldBe(Optional<String>.Some("a"));
    }

    [Fact]
    public void AsOptional_NullClass_ReturnsNone()
    {
        String? value = null;

        var optional = value.AsOptional();

        optional.ShouldBe(Optional<String>.None());
    }
}
