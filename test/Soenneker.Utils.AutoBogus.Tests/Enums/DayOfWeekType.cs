using Ardalis.SmartEnum;

namespace Soenneker.Utils.AutoBogus.Tests.Enums
{
    public sealed class DayOfWeekType : SmartEnum<DayOfWeekType>
    {
        public static readonly DayOfWeekType Sunday = new(nameof(Sunday), 0);
        public static readonly DayOfWeekType Monday = new(nameof(Monday), 1);
        public static readonly DayOfWeekType Tuesday = new(nameof(Tuesday), 2);
        public static readonly DayOfWeekType Wednesday = new(nameof(Wednesday), 3);
        public static readonly DayOfWeekType Thursday = new(nameof(Thursday), 4);
        public static readonly DayOfWeekType Friday = new(nameof(Friday), 5);
        public static readonly DayOfWeekType Saturday = new(nameof(Saturday), 6);

        private DayOfWeekType(string name, int value) : base(name, value)
        {
        }
    }
}
