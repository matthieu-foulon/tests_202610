using FluentAssertions;

namespace TestProject;

public class RingBufferEdgeCasesTests : RingBufferTests
{
    [Fact]
    public void GetAndPeek_ShouldReturnDefault_WhenBufferIsEmpty()
    {
        bool result_peek = _buffer.TryPeek(out var peek);
        result_peek.Should().BeFalse();

        bool result_get = _buffer.TryGet(out var get);
        result_get.Should().BeFalse();
    }

    [Fact]
    public void Put_ShouldReturnFalse_WhenBufferIsFull()
    {
        // fill at full capacity
        _buffer.Put(1).Should().BeTrue();
        _buffer.Put(2).Should().BeTrue();
        _buffer.Put(3).Should().BeTrue();

        // trying to add a 4th
        bool success = _buffer.Put(4);

        success.Should().BeFalse();
        _buffer.Length.Should().Be(3);
    }

    [Fact]
    public void Buffer_ShouldWrapAround_Correctly()
    {
        // fill at full capacity
        _buffer.Put(1);
        _buffer.Put(2);
        _buffer.Put(3);

        // remove 1st
        _buffer.TryGet(out int value).Should().BeTrue();
        value.Should().Be(1);

        // readd one
        bool success = _buffer.Put(4);

        success.Should().BeTrue();
        _buffer.Length.Should().Be(3);

        _buffer.TryGet(out value).Should().BeTrue();
        value.Should().Be(2);
        _buffer.TryGet(out value).Should().BeTrue();
        value.Should().Be(3);
        _buffer.TryGet(out value).Should().BeTrue();
        value.Should().Be(4);
    }
}
