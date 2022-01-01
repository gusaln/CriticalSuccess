namespace CriticalSuccess.Core;

/// <summary> Represents a modifier for a Dice. </summary>
public enum Modifier : byte
{
    /// <summary> Has no effect on the roll. </summary>
    Null,
    /// <summary>Only keeps the highest value. </summary>
    KeepHigh,
    /// <summary>Drops keeps the highest value. </summary>
    DropHigh,
    /// <summary>Only keeps the lowest value. </summary>
    KeepLow,
    /// <summary>Drops keeps the lowest value. </summary>
    DropLow,
    /// <summary>Any time the max value of a die is rolled, the die is rerolled and it both values are added to the total. </summary>
    Explode
}
