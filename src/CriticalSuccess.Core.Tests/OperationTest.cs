using System.Linq;
using Xunit;

namespace CriticalSuccess.Core.Tests;

public class OperationTest
{
    [Fact]
    public void ItReturnsTheResultOfAnOperation()
    {
        var addOperation = new Operation(Operator.Add, new Constant(3), new Constant(2));
        Assert.Equal(3 + 2, addOperation.Value());

        var subOperation = new Operation(Operator.Sub, new Constant(3), new Constant(2));
        Assert.Equal(3 - 2, subOperation.Value());
    }

    [Fact]
    public void ItReturnsTheResultOfEachExpressionAsSteps()
    {
        var const3 = new Constant(3);
        var const2 = new Constant(2);
        var operation = new Operation(Operator.Add, const3, const2);

        var result = operation.Result();
        Assert.Equal(2, result.Steps.Count());

        var firstStep = result.Steps.ElementAt(0);
        Assert.Equal(new Result(const3, 3), firstStep);

        var secondStep = result.Steps.ElementAt(1);
        Assert.Equal(new Result(const2, 2), secondStep);
    }
}
