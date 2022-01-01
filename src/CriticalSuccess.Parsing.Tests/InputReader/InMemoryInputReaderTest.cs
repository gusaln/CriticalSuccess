using CriticalSuccess.Parsing.InputReader;
using Xunit;

namespace CriticalSuccess.Parsing.Tests.InputReader;

public class InMemoryInputReaderTest
{
    // The test input does not have the same letter twice in different positions.
    public const string TEST_INPUT = "steak";

    public const byte TEST_INPUT_LEN = 5;

    protected IInputReader createReader() => new InMemoryInputReader(TEST_INPUT);

    protected string getTestInput() => TEST_INPUT;

    protected ushort getTestInputLength() => TEST_INPUT_LEN;

    [Fact]
    public void InMemoryInputReader_NextPos_InitialState_IsMinus1()
    {
        var reader = createReader();

        Assert.Equal(-1, reader.NextPos);
    }

    [Fact]
    public void InMemoryInputReader_IsSOF_InitialState_ReturnsTrue()
    {
        var reader = createReader();

        Assert.True(reader.IsSOF(), "Reader does not start at SOF state.");
    }

    [Fact]
    public void InMemoryInputReader_Peek_InitialState_ReturnsNull()
    {
        var reader = createReader();

        Assert.Null(reader.Peek());
    }

    [Fact]
    public void InMemoryInputReader_Peek_Always_DoesNotMoveTheCursor()
    {
        var reader = createReader();
        var pos = reader.NextPos;
        reader.Peek();

        Assert.Equal(pos, reader.NextPos);
    }

    [Fact]
    public void InMemoryInputReader_Consume_InitialState_ReturnsNull()
    {
        var reader = createReader();
        var pos = reader.NextPos;

        Assert.Null(reader.Consume());
    }

    [Fact]
    public void InMemoryInputReader_Consume_Always_MovesTheCursor()
    {
        var reader = createReader();
        var pos = reader.NextPos;

        reader.Consume();
        Assert.Equal(pos + 1, reader.NextPos);
    }


    [Fact]
    public void InMemoryInputReader_Consume_Always_RunsThroughTheInput()
    {
        var reader = createReader();

        // we consume the first character that is null by design.
        reader.Consume();

        foreach (var c in getTestInput().ToCharArray())
        {
            Assert.Equal(c, reader.Consume());
        }
    }

    [Fact]
    public void InMemoryInputReader_IsSOF_AfterInitialState_ReturnsFalse()
    {
        var reader = createReader();
        reader.Consume();

        Assert.False(reader.IsSOF(), "Reader did not leave SOF state.");
    }

    [Fact]
    public void InMemoryInputReader_Peek_Always_CanProccesNCharactersAheadWithoutMovingTheCursor()
    {
        var reader = createReader();
        var pos = reader.NextPos;

        Assert.Equal('s', reader.Peek(1));
        Assert.Equal(pos, reader.NextPos);

        Assert.Equal('k', reader.Peek(5));
        Assert.Equal(pos, reader.NextPos);
    }

    [Fact]
    public void InMemoryInputReader_Consume_Always_CanProccesNCharactersAheadWhileMovingTheCursor()
    {
        var reader = createReader();
        var pos = reader.NextPos;

        Assert.Equal('s', reader.Consume(1));
        Assert.Equal(pos + 2, reader.NextPos);

        pos = reader.NextPos;
        Assert.Equal('k', reader.Consume(3));
        Assert.Equal(pos + 4, reader.NextPos);
    }

    [Fact]
    public void InMemoryInputReader_IsEOF_WhenLastCharIsReached_ReturnsTrue()
    {
        var reader = createReader();
        reader.Consume(getTestInputLength());

        Assert.True(reader.IsEOF(), "Reader does not end at EOF state.");
        Assert.Equal(getTestInputLength(), reader.NextPos);
    }

    [Fact]
    public void InMemoryInputReader_Peek_AfterLastCharIsReached_ReturnsTrue()
    {
        var reader = createReader();
        reader.Consume(getTestInputLength());

        Assert.True(reader.IsEOF(), "Reader does not end at EOF state.");
        Assert.Null(reader.Peek());
    }

    [Fact]
    public void InMemoryInputReader_Consume_AfterLastCharIsReached_ReturnsTrue()
    {
        var reader = createReader();
        reader.Consume(getTestInputLength());

        Assert.True(reader.IsEOF(), "Reader does not end at EOF state.");
        Assert.Null(reader.Consume());
    }
}
