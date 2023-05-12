using System.Linq;
using Xunit;

namespace CriticalSuccess.Core.Tests;

public class DiceTest
{
    [Fact]
    public void ItReturnsAValueInTheRange()
    {
        var dice = new Dice(4);

        Assert.InRange(dice.Value(), 1, 4);
    }

    [Fact]
    public void ItHandlesNegativeFaces()
    {
        var dice = new Dice(-4);

        Assert.InRange(dice.Value(), -4, -1);
    }

    [Fact]
    public void ItHandlesZeroFace()
    {
        var dice = new Dice(24, 0);

        Assert.Equal(0, dice.Value());
    }

    [Fact]
    public void ItProducesCorrectResults()
    {
        byte numberOfDice = 3;
        int faces = 10;
        var dice3d10 = new Dice(numberOfDice, faces);

        var result = dice3d10.Result();
        Assert.Equal(dice3d10, result.Expression);
        Assert.Equal(numberOfDice, result.Steps.Count);
        Assert.Equal(result.Steps.Sum(r => r.Value), result.Value);

        numberOfDice = 1;
        var dice1d10 = new Dice(numberOfDice, faces);

        result = dice1d10.Result();
        Assert.Equal(dice1d10, result.Expression);
        Assert.Equal(1, result.Steps.Count);
    }

    [Fact]
    public void ItRespectsModifiers()
    {
        // The expected output for 3d6 is 5, 3 and 1. Adding any two numbers in the set will result
        // in a unique number that is also not in the set.
        Dice.SetSeed(2);

        Assert.Equal(5 + 3 + 1, new Dice(3, 6).Value());
        Assert.Equal(5, new Dice(3, 6, Modifier.KeepHigh).Value());
        Assert.Equal(3 + 1, new Dice(3, 6, Modifier.DropHigh).Value());
        Assert.Equal(1, new Dice(3, 6, Modifier.KeepLow).Value());
        Assert.Equal(5 + 3, new Dice(3, 6, Modifier.DropLow).Value());

        // The expected output for d4's is 2, 3, 4 and 1.
        Dice.SetSeed(3);
        Assert.Equal(2 + 3 + 4, new Dice(3, 4).Value());
        Assert.Equal(2 + 3 + 4 + 1, new Dice(3, 4, Modifier.Explode).Value());
    }
}
