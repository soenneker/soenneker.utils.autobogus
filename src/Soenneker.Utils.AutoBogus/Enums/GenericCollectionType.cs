namespace Soenneker.Utils.AutoBogus.Enums;

/// <summary>
/// Represents the type of generic collection.
/// </summary>
public enum GenericCollectionType
{
    /// <summary>
    /// Unknown collection type.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// An enumerable collection.
    /// </summary>
    Enumerable = 1,

    /// <summary>
    /// A collection.
    /// </summary>
    Collection = 2,

    /// <summary>
    /// A read-only collection.
    /// </summary>
    ReadOnlyCollection = 3,

    /// <summary>
    /// A set.
    /// </summary>
    Set = 4,

    /// <summary>
    /// A list.
    /// </summary>
    List = 5,

    /// <summary>
    /// A read-only list.
    /// </summary>
    ReadOnlyList = 6,

    /// <summary>
    /// A sorted list.
    /// </summary>
    SortedList = 7,

    /// <summary>
    /// A read-only dictionary.
    /// </summary>
    ReadOnlyDictionary = 8,

    /// <summary>
    /// An immutable dictionary.
    /// </summary>
    ImmutableDictionary = 9,

    /// <summary>
    /// A dictionary.
    /// </summary>
    Dictionary = 10,

    /// <summary>
    /// An immutable list.
    /// </summary>
    ImmutableList = 11,

    /// <summary>
    /// An immutable array.
    /// </summary>
    ImmutableArray = 12
}