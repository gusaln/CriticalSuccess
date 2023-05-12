# CriticalSuccess

This is a simple experimental parser for dice expressions like `1d20`, `4d6` and `5d8`.

The parser currently supports normal dice rolls, scalar numbers, operations (only addition and subtraction for now) and modifiers (keep the highest, drop the lowest, etc).

While not in active development, I share this code hoping you find this concept interesting.

## Table of contents

[TOC]

## Installation

<!-- TODO -->

## Basic usage

You can parse [expressions](#details-on-the-expressions-syntax) like `1d20` using the cli and get a result.

```shell
# usage
# criticalSuccess <roll> [-p|--prefix]...

criticalSuccess 1d20
# 15
```

If the expression has spaces, you have to wrap them in `"` like

```shell
criticalSuccess "3d6 + 1"
# 14
```

You can roll multiple sets of dice by giving multiple expressions separated by spaces or using the a semi-colon symbol `;`.
The results will appear line by line.

```shell
# you can use spaces
# criticalSuccess 1d20 "d6 + 1" 5d8
# or semi-colons
# criticalSuccess "1d20 ; d6 + 1 ; 5d8"
# or both
# criticalSuccess "1d20 ; d6 + 1" 5d8
# all of the above are equivalent:

criticalSuccess 1d20 "d6 + 1" 5d8
# 3
# 4
# 22
```

You could also use the `;` symbol to separate expressions in a single string.

```shell
criticalSuccess "1d20 ; d6 + 1"
# equivalent to criticalSuccess "1d20 ; d6 + 1"
# 3
# 4
```

By giving the `-p|--prefix` option (it has to be at the end), each result will appear prefixed by its expressions.

```shell
criticalSuccess "2d20H + 1d4" 3d6 -p
# 2d20H + 1d4: 21
# 3d6: 10
```

If there is an error in any of the expressions, you will get best description of the problem.

```shell
criticalSuccess dd
# Syntax error at position 1: number in range [-2147483648, 2147483647] expected, but found DSymbol.
# dd
#  ^
```

The command can spawn an interactive shell using the option cli.

```shell
criticalSuccess cli
Enter an expression or a command and press enter
Commands: [c] to close the shell, [?] to print this message
> 2d10
# 9
> 5d8
# 21
> c
# exits
# you can also press Ctrl + C to exit
```

### Quick reference of how to express dice rolls

Below is a table establishing desired rolls and their expressions.

| Desired roll                                                           | Expressions                 |
| :--------------------------------------------------------------------- | :-------------------------- |
| one d20                                                                | `1d20`, `d20`               |
| four d6                                                                | `4d6`                       |
| four d6, **dropping the lowest**                                       | `4d6l`                      |
| two d20, **keeping the lowest**                                        | `2d20L`                     |
| three d4, **dropping the highest**                                     | `3d4h`                      |
| two d20, **keeping the highest**                                       | `2d20H`                     |
| two d6, reroll every die that comes up 1 or 6, even if it was a reroll | `2d6e`                      |
| one d20, one d4 and sum the results                                    | `1d20 + 1d4`, `d20 + d4`    |
| two d20, **keeping the highest**, one d4 and sum the results           | `2d20H + 1d4`, `2d20H + d4` |
| three d6 plus 2                                                        | `3d6 + 2`                   |
| three d6 plus 2 plus 1                                                 | `3d6 + 2 + 1`               |
| one d20 minus 2                                                        | `1d20 - 2`, `d20 - 2`       |
| just give me 2 + 1                                                     | `2 + 1`                     |
| one d20 and separately three d4                                        | `d20 ; 3d4`                 |

### Details on the expressions syntax

The syntax is meant to read dice rolls like you would find in _TTRPGs_ (Table Top Roll Playing Games).
Therefore, a string of characters like `4d6` means _roll four six-sided dice and sum the results_.

The parser accepts any expression of the form `[number of dice]d<number of sides>[modifier]` where `[number of dice]` is a number between 0 and 255 (inclusive), `<number of sides>` is a number between -2147483648 and 2147483647 (also inclusive) and `[modifier]` changes the way in the results are added up.
In case `[number of dice]` is 1, you can simplify the expression to `d<number of sides>`.
You can read more about [dice with a negative number of sides here](#negative-dice)) and check the [available modifiers below](#modifiers).

Other considerations:

- you can add or subtract any scalar number for a die (`1d6 + 1` will roll one d6 and add 1 to the result).
- the grammar ignores spaces, so `1 d 20` and `2 + 1` are equivalent to `1d20` and `2+1` respectively.
- the _d_ in any dice expression is case insensitive, meaning `d20` and `4d6` are equivalent to `D20` and `4D6` respectively.
- You can separate expressions using the special symbol `;`.

### Modifiers

At the end of any dice expression, you can use one of the following:

- **H** only keeps the highest result.
    If you roll any number of dice, only the highest value will be kept.
- **L** only keeps the lowest result.
    If you roll any number of dice, only the lowest value will be kept.
- **h** drops the highest result.
    If you roll any number of dice, the highest will be ignored.
- **l** drops the lowest result.
    If you roll any number of dice, the lowest will be ignored.
- **e** explotes any single die in the result.
    For any number of dice you roll if you get the lowest or highest value possible on that die, roll an additional die of the same type.
    Note that the rule applies for any _additional die_ you roll, so in theory you could get an infinite loop.:

### Formal grammar

The formal grammar can be found in [this document](src/CriticalSuccess.Parsing/grammar.txt).

### Negative dice

You can write expressions like `1d-4` and you will get a number between -4 and -1.
It is equivalent to writing `0 - 1d4`.

## Project structure

The whole project is develop in `C#`.
The `src` folder holds the `Solution` and the `Projects` with the components of the applications.

| Project                         | Description                 |
| :------------------------------ | :-------------------------- |
| `CriticalSuccess.Core`          | Core logic for dice rolling |
| `CriticalSuccess.Core.Tests`    | Core logic tests            |
| `CriticalSuccess.Parsing`       | Parser and code generator   |
| `CriticalSuccess.Parsing.Tests` | Parser tests                |
| `CriticalSuccess.Console`       | The cli                     |

## License

[MIT License](LICENSE)
