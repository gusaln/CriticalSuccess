using System;
using Xunit;

namespace CriticalSuccess.Core.Tests;

public class RollsTest
{
    [Fact]
    public void ItReturnsTheResultsOfTheExpressions()
    {
        var rng = new Random();
        var const1 = rng.Next();
        var const2 = rng.Next();
        var rolls = new Rolls(new IExpression[] { new Constant(const1), new Constant(const2) });

        Assert.Equal(
            new Result[] {
                    new Result(new Constant(const1), const1),
                    new Result(new Constant(const2), const2),
            },
            rolls.Results()
        );
    }

    [Fact]
    public void ItReturnsTheValuesOfTheExpressions()
    {
        var rng = new Random();
        var const1 = rng.Next();
        var const2 = rng.Next();
        var rolls = new Rolls(new IExpression[] { new Constant(const1), new Constant(const2) });

        Assert.Equal(
            new int[] {
                const1,
                const2,
            },
            rolls.Values()
        );
    }
}
