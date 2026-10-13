using FluentAssertions;

namespace TestProject;

public class RingBufferOperationsTests : RingBufferTests
{
    [Fact]
    public void Put_ShouldAddElement_AndChangeState()
    {
        bool success = _buffer.Put(42);

        success.Should().BeTrue();
        _buffer.Length.Should().Be(1);
        _buffer.Empty.Should().BeFalse();
    }

    [Fact]
    public void Peek_ShouldReturnFirstElement_WithoutRemovingIt()
    {
        _buffer.Put(10);
        _buffer.Put(20);

         _buffer.TryPeek(out int peekedValue).Should().BeTrue();

        peekedValue.Should().Be(10);
        _buffer.Length.Should().Be(2);
    }

    [Fact]
    public void Get_ShouldReturnFirstElement_AndRemoveIt()
    {
        _buffer.Put(10);
        _buffer.Put(20);

        _buffer.TryGet(out int firstGet).Should().BeTrue();
        _buffer.TryGet(out int secondGet).Should().BeTrue();

        firstGet.Should().Be(10);
        secondGet.Should().Be(20);
        _buffer.Length.Should().Be(0);
        _buffer.Empty.Should().BeTrue();
    }
}
