using Intellenum;

namespace Soenneker.Utils.AutoBogus.Tests.Enums;

[Intellenum<string>]
public partial class DayOfWeekTypeIntellenum
{
    public static readonly DayOfWeekTypeIntellenum Sunday = new(nameof(Sunday));
    public static readonly DayOfWeekTypeIntellenum Monday = new(nameof(Monday));
    public static readonly DayOfWeekTypeIntellenum Tuesday = new(nameof(Tuesday));
    public static readonly DayOfWeekTypeIntellenum Wednesday = new(nameof(Wednesday));
    public static readonly DayOfWeekTypeIntellenum Thursday = new(nameof(Thursday));
    public static readonly DayOfWeekTypeIntellenum Friday = new(nameof(Friday));
    public static readonly DayOfWeekTypeIntellenum Saturday = new(nameof(Saturday));
}