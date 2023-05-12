using System;

namespace CriticalSuccess.Core.Serialization
{
    abstract public record IExpressionDto
    {
        abstract public string Type { get; }
    }
}
