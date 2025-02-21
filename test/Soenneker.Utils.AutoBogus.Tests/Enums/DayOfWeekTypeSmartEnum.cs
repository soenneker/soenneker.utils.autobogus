using Ardalis.SmartEnum;

namespace Soenneker.Utils.AutoBogus.Tests.Enums;

public sealed class DayOfWeekTypeSmartEnum : SmartEnum<DayOfWeekTypeSmartEnum>
{
    public static readonly DayOfWeekTypeSmartEnum Sunday = new(nameof(Sunday), 0);
    public static readonly DayOfWeekTypeSmartEnum Monday = new(nameof(Monday), 1);
    public static readonly DayOfWeekTypeSmartEnum Tuesday = new(nameof(Tuesday), 2);
    public static readonly DayOfWeekTypeSmartEnum Wednesday = new(nameof(Wednesday), 3);
    public static readonly DayOfWeekTypeSmartEnum Thursday = new(nameof(Thursday), 4);
    public static readonly DayOfWeekTypeSmartEnum Friday = new(nameof(Friday), 5);
    public static readonly DayOfWeekTypeSmartEnum Saturday = new(nameof(Saturday), 6);

    private DayOfWeekTypeSmartEnum(string name, int value) : base(name, value)
    {
    }
}