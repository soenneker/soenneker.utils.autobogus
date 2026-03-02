using Soenneker.Gen.EnumValues;

namespace Soenneker.Utils.AutoBogus.Tests.Enums;

[EnumValue]
public sealed partial class OrderStatusEnumValue
{
    public static readonly OrderStatusEnumValue Pending = new(1);
    public static readonly OrderStatusEnumValue Completed = new(2);
    public static readonly OrderStatusEnumValue Cancelled = new(3);
}
