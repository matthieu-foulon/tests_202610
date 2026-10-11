using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;

public class RingBufferOperationsTests : RingBufferTests
{
    [Fact]
    public void Put_ShouldAddElement_AndChangeState()
    {
        // Act
        bool success = _buffer.Put(42);

        // Assert
        success.Should().BeTrue();
        _buffer.Length.Should().Be(1);
        _buffer.Empty.Should().BeFalse();
    }

    [Fact]
    public void Peek_ShouldReturnFirstElement_WithoutRemovingIt()
    {
        // Arrange
        _buffer.Put(10);
        _buffer.Put(20);

        // Act
         _buffer.TryPeek(out int peekedValue).Should().BeTrue();

        // Assert
        peekedValue.Should().Be(10);
        _buffer.Length.Should().Be(2); // La taille n'a pas bougé
    }

    [Fact]
    public void Get_ShouldReturnFirstElement_AndRemoveIt()
    {
        // Arrange
        _buffer.Put(10);
        _buffer.Put(20);

        // Act
        _buffer.TryGet(out int firstGet).Should().BeTrue();
        _buffer.TryGet(out int secondGet).Should().BeTrue();

        // Assert
        firstGet.Should().Be(10);
        secondGet.Should().Be(20);
        _buffer.Length.Should().Be(0);
        _buffer.Empty.Should().BeTrue();
    }
}
