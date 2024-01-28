using Ardalis.SmartEnum;
#pragma warning disable CA2211

namespace Soenneker.Utils.AutoBogus.Enums;

public sealed class GenericCollectionType : SmartEnum<GenericCollectionType>
{
    public static GenericCollectionType Dictionary = new(nameof(Dictionary), 10);
    public static GenericCollectionType ImmutableDictionary = new(nameof(ImmutableDictionary), 9);
    public static GenericCollectionType ReadOnlyDictionary = new(nameof(ReadOnlyDictionary), 8);
    public static GenericCollectionType SortedList = new(nameof(SortedList), 7);
    public static GenericCollectionType ReadOnlyList = new(nameof(ReadOnlyList), 6);
    public static GenericCollectionType ListType = new(nameof(ListType), 5);
    public static GenericCollectionType Set = new(nameof(Set), 4);
    public static GenericCollectionType ReadOnlyCollection = new(nameof(ReadOnlyCollection), 3);
    public static GenericCollectionType Collection = new(nameof(Collection), 2);
    public static GenericCollectionType Enumerable = new(nameof(Enumerable), 1);
    public static GenericCollectionType Unknown = new(nameof(Unknown), 0);

    private GenericCollectionType(string name, int value) : base(name, value)
    {
    }
}