using System;
using Xunit;

namespace CriticalSuccess.Core.Tests;

public class ConstantTest
{
    [Fact]
    public void ItReturnsTheValueItGets()
    {
        var value = new Random().Next();

        IExpression constant = new Constant(value);

        Assert.Equal(value, constant.Value());
    }

    [Fact]
    public void ItProducesAValidResult()
    {
        var value = new Random().Next();
        var constant = new Constant(value);

        var expectedResult = new Result(constant, value);

        Assert.Equal(expectedResult, constant.Result());
    }
}
