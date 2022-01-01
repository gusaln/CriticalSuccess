using System.Collections.Generic;
using Xunit;

namespace CriticalSuccess.Core.Tests;

internal struct MockExpression : IExpression
{
    public Result Result()
    {
        return new Result(this, 1);
    }
}

internal struct MockExpressionWithProcess : IExpression
{
    public Result Result()
    {
        return new Result(
            this,
            1,
            new List<Result>(new Result[] { new Result(this, 1) })
        );
    }
}

public class ResultTest
{
    [Fact]
    public void ItEqualsEquivalentResults()
    {
        var mockExpression = new MockExpression();
        var expectedResult = new Result(mockExpression, 1);
        Assert.Equal(expectedResult, mockExpression.Result());

        var mockExpressionWithProcess = new MockExpressionWithProcess();
        expectedResult = new Result(
            mockExpressionWithProcess,
            1,
            new List<Result>(new Result[] { new Result(mockExpressionWithProcess, 1) })
        );
        Assert.Equal(expectedResult, mockExpressionWithProcess.Result());
    }
}
