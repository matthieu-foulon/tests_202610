using FluentAssertions;

public class RingBufferEdgeCasesTests : RingBufferTests
{
    [Fact]
    public void GetAndPeek_ShouldReturnDefault_WhenBufferIsEmpty()
    {
        // Act & Assert
        bool result_peek = _buffer.TryPeek(out var peek);
        result_peek.Should().BeFalse();

        bool result_get = _buffer.TryGet(out var get);
        result_get.Should().BeFalse();
    }

    [Fact]
    public void Put_ShouldReturnFalse_WhenBufferIsFull()
    {
        // Arrange : On remplit le buffer jusqu'à sa capacité (3)
        _buffer.Put(1).Should().BeTrue();
        _buffer.Put(2).Should().BeTrue();
        _buffer.Put(3).Should().BeTrue();

        // Act : On tente d'ajouter un 4ème élément
        bool success = _buffer.Put(4);

        // Assert
        success.Should().BeFalse();
        _buffer.Length.Should().Be(3); // Reste bloqué au maximum
    }

    [Fact]
    public void Buffer_ShouldWrapAround_Correctly()
    {
        // Arrange : On remplit
        _buffer.Put(1);
        _buffer.Put(2);
        _buffer.Put(3);

        // On libère une place en lisant le premier élément
        _buffer.TryGet(out int value).Should().BeTrue();
        value.Should().Be(1);

        // Act : On ajoute un nouvel élément (il doit se mettre au début du tableau sous le capot)
        bool success = _buffer.Put(4);

        // Assert
        success.Should().BeTrue();
        _buffer.Length.Should().Be(3);

        // L'ordre de lecture doit rester FIFO (First In, First Out)
        _buffer.TryGet(out value).Should().BeTrue();
        value.Should().Be(2);
        _buffer.TryGet(out value).Should().BeTrue();
        value.Should().Be(3);
        _buffer.TryGet(out value).Should().BeTrue();
        value.Should().Be(4);
    }
}
