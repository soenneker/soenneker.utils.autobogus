#pragma warning disable CA2211

namespace Soenneker.Utils.AutoBogus.Enums;

public enum GenericCollectionType
{
    Unknown = 0,
    Enumerable = 1,
    Collection = 2,
    ReadOnlyCollection = 3,
    Set = 4,
    ListType = 5,
    ReadOnlyList = 6,
    SortedList = 7,
    ReadOnlyDictionary = 8,
    ImmutableDictionary = 9,
    Dictionary = 10
}