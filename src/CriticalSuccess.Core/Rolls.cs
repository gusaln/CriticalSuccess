using System.Collections.Generic;
using System.Linq;

namespace CriticalSuccess.Core;

/// <summary>
/// Represents a collections of <see cref="IExpression">IExpression</see>.
/// </summary>
public class Rolls
{
    private List<IExpression> _expressions;

    /// <summary>
    /// The collection of <see cref="IExpression">IExpression</see>'s stored.
    /// </summary>
    public IReadOnlyCollection<IExpression> Expressions => _expressions.AsReadOnly();

    /// <summary>
    /// Creates a <c>Rolls</c> instance that contains single <see cref="IExpression">IExpression</see>.
    /// </summary>
    /// <param name="expression"> An <see cref="IExpression">IExpression</see>. </param>
    public Rolls(IExpression expression)
    {
        _expressions = new List<IExpression>(new IExpression[] { expression });
    }

    /// <summary>
    /// Creates a <c>Rolls</c> instance that contains N <see cref="IExpression">IExpression</see>s.
    /// </summary>
    /// <param name="expressions"> A collection of <see cref="IExpression">IExpression</see>s. </param>
    public Rolls(IEnumerable<IExpression> expressions)
    {
        _expressions = new List<IExpression>(expressions);
    }

    /// <summary>
    /// Returns a collection of values produce from the <see cref="IExpression">IExpression</see>s.
    /// </summary>
    /// <returns> A collection of <c>int</c>s. </returns>
    public IEnumerable<int> Values()
    {
        return _expressions.Select(expr => expr.Value());
    }

    /// <summary>
    /// Returns a collection of <c>Result</c>s produce from the <see cref="IExpression">IExpression</see>s.
    /// </summary>
    /// <returns> A collection of <c>Result</c>s. </returns>
    public IEnumerable<Result> Results()
    {
        return _expressions.Select(expr => expr.Result());
    }
}
