using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CriticalSuccess.Core.Serialization
{
    /// <summary>
    /// Facilitates the mapping Modifiers to and from their string representation.
    /// </summary>
    public static class ModifierMapper
    {
        /// <summary> Map chars to <c>Modifier</c>. </summary>
        public static ReadOnlyDictionary<char, Modifier> FromChar
        {
            get => new ReadOnlyDictionary<char, Modifier>(
                new Dictionary<char, Modifier>()
                {
                    ['H'] = Modifier.KeepHigh,
                    ['L'] = Modifier.KeepLow,
                    ['h'] = Modifier.DropHigh,
                    ['l'] = Modifier.DropLow,
                    ['e'] = Modifier.Explode,
                }
            );
        }

        /// <summary> Map <c>Modifier</c> to chars. </summary>
        public static ReadOnlyDictionary<Modifier, char> ToChar
        {
            get => new ReadOnlyDictionary<Modifier, char>(
                new Dictionary<Modifier, char>()
                {
                    [Modifier.KeepHigh] = 'H',
                    [Modifier.KeepLow] = 'L',
                    [Modifier.DropHigh] = 'h',
                    [Modifier.DropLow] = 'l',
                    [Modifier.Explode] = 'e',
                }
            );
        }
    }
}