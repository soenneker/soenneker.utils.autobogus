using System.Collections.Generic;
using System.Linq;
using Bogus;
using AwesomeAssertions;
using Soenneker.Utils.AutoBogus.Config;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Extensions;
using Xunit;

namespace Soenneker.Utils.AutoBogus.Tests;

public class AutoGenerateContextFixture
{
    private readonly Faker _faker;
    private readonly HashSet<string> _ruleSets;
    private readonly AutoFakerConfig _fakerConfig;
    private AutoFakerContext _context;

    public AutoGenerateContextFixture()
    {
        _faker = new Faker();
        _ruleSets = [];
        _fakerConfig = new AutoFakerConfig();
    }

    public class GenerateMany_Internal: AutoGenerateContextFixture
    {
        private int _value;
        private readonly List<int> _items;

        public GenerateMany_Internal()
        {
            _value = _faker.Random.Int();
            _items = [_value];

            var autoFaker = new AutoFaker();

            _context = new AutoFakerContext(autoFaker)
            {
                RuleSets = _ruleSets
            };
        }

        [Fact]
        public void Should_Generate_Configured_RepeatCount()
        {
            int count = _faker.Random.Int(3, 5);
            List<int> expected = Enumerable.Range(0, count).Select(i => _value).ToList();

            _fakerConfig.RepeatCount = count;

            List<int>? items = AutoGenerateContextExtension.GenerateMany(_context, count, false, 1, () => _value);

            items.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void Should_Generate_Duplicates_If_Not_Unique()
        {
            List<int>? items = AutoGenerateContextExtension.GenerateMany(_context, 2, false, 1, () => _value);

            items.Should().BeEquivalentTo(new[] {_value, _value});
        }

        [Fact]
        public void Should_Not_Generate_Duplicates_If_Unique()
        {
            int first = _value;
            int second = _faker.Random.Int();

            List<int>? items = AutoGenerateContextExtension.GenerateMany(_context, 2, true, 1, () =>
            {
                int item = _value;
                _value = second;

                return item;
            });

            items.Should().BeEquivalentTo(new[] {first, second});
        }

        [Fact]
        public void Should_Short_Circuit_If_Unique_Attempts_Overflow()
        {
            var attempts = 0;

            List<int>? items = AutoGenerateContextExtension.GenerateMany(_context, 2, true, AutoFakerDefaultConfigOptions.GenerateAttemptsThreshold, () =>
            {
                attempts++;
                return _value;
            });

            attempts.Should().Be(AutoFakerDefaultConfigOptions.GenerateAttemptsThreshold + 1);

            items.Should().BeEquivalentTo(new[] {_value});
        }
    }
}