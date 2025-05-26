using System;
using Bogus;
using AwesomeAssertions;
using Xunit;

namespace Soenneker.Utils.AutoBogus.Tests.Playground;

public class NodaTimeFixture
{
    public sealed class TimeHolder
    {
        private DateTime _time;
        private bool _unstable;

        public DateTime Time
        {
            get => _time;
            set
            {
                if (value.Day != 2 && value.Month != 2)
                {
                    _unstable = true;
                }

                if (_unstable)
                {
                    throw new Exception();
                }

                _time = value;
            }
        }
    }

    public class TestValidValueCreation
    {
        private readonly DateTime _validDate = new DateTime(2020, 2, 2);

        [Fact]
        public void TestFaker()
        {
            TimeHolder? created = new Faker<TimeHolder>()
                .RuleFor(x => x.Time, faker => _validDate)
                .Generate();

            created.Should().NotBeNull();
        }

        [Fact]
        public void TestAutoFaker()
        {
            Faker<TimeHolder>? faker = new Faker<TimeHolder>().RuleFor(x => x.Time, faker => _validDate);

            TimeHolder? created = faker.Generate();
            created = faker.Generate();

            created.Should().NotBeNull();
        }
    }
}