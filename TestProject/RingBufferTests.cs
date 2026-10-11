using FluentAssertions;
using hsbc_test_20260930;
using System;
using System.Collections.Generic;
using System.Text;

public class RingBufferTests
{
    protected readonly IRingBuffer<int> _buffer;
    protected const int BufferCapacity = 3;

    public RingBufferTests()
    {
        // Remplacez par votre classe concrète (ex: MyRingBuffer)
        _buffer = new RingBuffer<int>(BufferCapacity);
    }

    [Fact]
    public void Constructor_ShouldInitializeCorrectly()
    {
        // Assert
        _buffer.Capacity.Should().Be(BufferCapacity);
        _buffer.Length.Should().Be(0);
        _buffer.Empty.Should().BeTrue();
    }
}
