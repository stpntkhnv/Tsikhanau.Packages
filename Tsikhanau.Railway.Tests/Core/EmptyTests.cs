namespace Tsikhanau.Railway.Tests;

public class EmptyTests
{
    [Fact]
    public void String_Constant_IsEmptyString()
    {
        var value = Empty.String;

        value.ShouldBe(String.Empty);
    }

    [Fact]
    public void Guid_Field_IsEmptyGuid()
    {
        var value = Empty.Guid;

        value.ShouldBe(Guid.Empty);
    }

    [Fact]
    public void Array_AnyType_ReturnsSharedEmptyArray()
    {
        var array = Empty.Array<Int32>();

        array.ShouldBeEmpty();
        array.ShouldBeSameAs(Array.Empty<Int32>());
    }
}
