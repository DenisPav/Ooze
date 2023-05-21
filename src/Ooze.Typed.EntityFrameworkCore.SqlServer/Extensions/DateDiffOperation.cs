namespace Ooze.Typed.EntityFrameworkCore.SqlServer.Extensions;

/// <summary>
/// Defines operation over DateDiff filters
/// </summary>
public enum DateDiffOperation
{
    /// <summary>
    /// Greater than diff
    /// </summary>
    GreaterThan,
    /// <summary>
    /// Less than diff
    /// </summary>
    LessThan,
    /// <summary>
    /// Equal diff
    /// </summary>
    Equal,
    /// <summary>
    /// Not equal diff
    /// </summary>
    NotEqual,
    /// <summary>
    /// Not greater than diff
    /// </summary>
    NotGreaterThan,
    /// <summary>
    /// Not less than diff
    /// </summary>
    NotLessThan,
}