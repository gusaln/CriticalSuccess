using System.Collections.Generic;
using System;
using System.Linq;

namespace CriticalSuccess.Core;

/// <summary>
/// Represents a the roll of N dice of F faces with an optional modifier.
/// </summary>
public record Dice : IExpression
{
    /// <summary> Number of dice. </summary>
    public readonly byte Number;

    /// <summary> Number of faces on each die. </summary>
    public readonly int NFaces;

    /// <summary> Roll modifier. </summary>
    public readonly Modifier Modifier;

    /// <summary> If set, the value is used as a seed for generating the values </summary>
    private static int? _seed = null;

    /// <summary>
    /// Creates a dice roll compose of a single die of the given number of <c>faces</c> with no modifier.
    /// </summary>
    /// <param name="nFaces">Number of faces on the die.</param>
    public Dice(int nFaces)
    {
        Number = 1;
        NFaces = nFaces;
        Modifier = Modifier.Null;
    }

    /// <summary>
    /// Creates a dice roll compose of a <c>number</c> die of the given number of <c>faces</c> with no modifier.
    /// </summary>
    /// <param name="number">Number of dice.</param>
    /// <param name="nFaces">Number of faces of the dice.</param>
    public Dice(byte number, int nFaces)
    {
        Number = number;
        NFaces = nFaces;
        Modifier = Modifier.Null;
    }

    /// <summary>
    /// Creates a dice roll compose of a <c>number</c> die of the given number of <c>faces</c> with a modifier.
    /// </summary>
    /// <param name="number">Number of dice.</param>
    /// <param name="nFaces">Number of faces of the dice.</param>
    /// <param name="modifier">The modifier for thw roll.</param>
    public Dice(byte number, int nFaces, Modifier modifier)
    {
        Number = number;
        NFaces = nFaces;
        Modifier = modifier;
    }

    /// <summary>
    /// Generates a value for the dice roll.
    /// </summary>
    /// <returns>A <c>Result</c> with the value generated and the steps of the process.</returns>
    public Result Result()
    {
        var rolls = rollDice();

        int result = Modifier switch
        {
            Modifier.KeepHigh => rolls.OrderBy(r => r).Last(),
            Modifier.KeepLow => rolls.OrderBy(r => r).First(),
            Modifier.DropHigh => rolls.OrderBy(r => r).SkipLast(1).Sum(),
            Modifier.DropLow => rolls.OrderBy(r => r).Skip(1).Sum(),
            _ => rolls.Sum(),
        };

        var diceRoll = this;
        return new Result(this, result, rolls.Select((value, index) => ConstantDie.FromValue(diceRoll.NFaces, value)));
    }

    /// <summary>
    /// Gets a value from the expression.
    /// </summary>
    /// <remarks> Multiple calls to this method could return different values. </remarks>
    /// <returns> The value from the expression. </returns>
    public int Value()
    {
        return Result().Value;
    }

    /// <summary>
    /// Generates the values for each die in the expression.
    /// </summary>
    /// <returns>An array with the results of 'rolling' of each die.</returns>
    private int[] rollDice()
    {
        (int Lower, int UpperPlusOne) bounds = NFaces switch
        {
            (> 0) => (1, NFaces + 1),
            (< 0) => (NFaces, 0),
            _ => (0, 0)
        };

        if (bounds == (0, 0))
        {
            return rollZeros();
        }

        if (Modifier == Modifier.Explode)
        {
            return rollExplodingMaxValues(bounds);
        }

        return rollStandard(bounds);
    }

    /// <summary>
    /// Generates an array where every element is zero.
    /// </summary>
    /// <returns>An array with <c>Number</c> zeros.</returns>
    private int[] rollZeros()
    {
        var rolls = new int[Number];

        for (var i = 0; i < Number; i++)
        {
            rolls[i] = 0;
        }

        return rolls;
    }

    /// <summary>
    /// Generates numbers for each die, rerolling max values.
    /// </summary>
    /// <param name="bounds">The bounds of the numbers rolled.</param>
    /// <returns>An array with at least <c>Number</c> numbers randomly generated.</returns>
    private int[] rollExplodingMaxValues((int Lower, int UpperPlusOne) bounds)
    {
        var rolls = new List<int>(Number * 2);

        int lastRolled = bounds.Lower;
        int max = bounds.UpperPlusOne - 1;
        var rng = getRandomGenerator();
        for (var i = 0; i < Number || lastRolled == max; i++)
        {
            rolls.Add(lastRolled = rng.Next(bounds.Lower, bounds.UpperPlusOne));
        }

        return rolls.ToArray();
    }

    /// <summary>
    /// Generates numbers for each die the standard way.
    /// </summary>
    /// <param name="bounds">The bounds of the numbers rolled.</param>
    /// <returns>An array with <c>Number</c> numbers randomly generated.</returns>
    private int[] rollStandard((int Lower, int UpperPlusOne) bounds)
    {
        var rolls = new int[Number];
        var rng = getRandomGenerator();
        for (var i = 0; i < Number; i++)
        {
            rolls[i] = rng.Next(bounds.Lower, bounds.UpperPlusOne);
        }

        return rolls;
    }

    /// <summary>
    /// Creates the random number generator.
    /// </summary>
    /// <returns>A seeded random number generator.</returns>
    private Random getRandomGenerator()
    {
        return _seed != null ? new Random((int)_seed) : new Random();
    }

    /// <summary>
    /// Generates the string representation of the dice roll.
    /// </summary>
    /// <returns>The string representation.</returns>
    public override string ToString()
    {
        return $"{Number}d{NFaces}" + Modifier switch
        {
            Modifier.Null => "",
            Modifier.KeepHigh => "H",
            Modifier.KeepLow => "L",
            Modifier.DropHigh => "h",
            Modifier.DropLow => "l",
            Modifier.Explode => "e",
            _ => throw new Exception($"Unkown Modifier {nameof(Modifier)}")
        };
    }

    /// <summary>
    /// Assigns the seed for the random number generator.
    /// </summary>
    /// <param name="seed">The seed to assign or <c>null</c> to reset the value.</param>
    public static void SetSeed(int? seed)
    {
        _seed = seed;
    }

    /// <summary>
    /// Represents the result of a single die roll.
    /// </summary>
    ///
    /// <param name="NFaces"> Number of faces of the die. </param>
    /// <param name="Value"> Value of the die. </param>
    public record ConstantDie(int NFaces, int Value) : IExpression
    {
        /// <summary>
        /// Represents the result of a single die roll.
        /// </summary>
        /// <param name="nFaces"> Number of faces of the die. </param>
        /// <param name="value"> Value rolled. </param>
        /// <returns> A <c>Result</c> with the value of the die. </returns>
        public static Result FromValue(int nFaces, int value)
        {
            return new ConstantDie(nFaces, value).Result();
        }

        /// <summary>
        /// Get a result.
        /// </summary>
        /// <returns> A <c>Result</c> with the value of the die. </returns>
        public Result Result()
        {
            return new Result(this, Value);
        }

        /// <summary>
        /// Generates the string representation of the die.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString()
        {
            return $"d{NFaces}";
        }
    }
}
