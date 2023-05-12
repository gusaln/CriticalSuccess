using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CriticalSuccess.Core.Serialization
{
    /// <summary>
    /// Facilitates the mapping Operators to and from their string representation.
    /// </summary>
    public static class OperatorMapper
    {
        /// <summary> Map chars to <c>Operator</c>. </summary>
        public static ReadOnlyDictionary<char, Operator> FromChar
        {
            get => new ReadOnlyDictionary<char, Operator>(
                new Dictionary<char, Operator>()
                {
                    ['+'] = Operator.Add,
                    ['-'] = Operator.Sub,
                }
            );
        }
        /// <summary> Map <c>Operator</c> to chars. </summary>
        public static ReadOnlyDictionary<Operator, char> ToChar
        {
            get => new ReadOnlyDictionary<Operator, char>(
                new Dictionary<Operator, char>()
                {
                    [Operator.Add] = '+',
                    [Operator.Sub] = '-',
                }
            );
        }
    }
}