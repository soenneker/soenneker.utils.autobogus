using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace Soenneker.Utils.AutoBogus.Tests.Playground;

public class CollectionsFixture
{
    private class Collections
    {
        public ICollection<string> C1 { get; set; }
        public IDictionary<string, string> C2 { get; set; }
        public IEnumerable<string> C3 { get; set; }
        public IList<string> C4 { get; set; }
        public IReadOnlyCollection<string> C5 { get; set; }
        public IReadOnlyDictionary<string, string> C6 { get; set; }
        public IReadOnlyList<string> C7 { get; set; }
        public ISet<string> C8 { get; set; }
    }
    
    [Fact]
    public void Should_Generate_Collections()
    {
        var c1 = AutoFaker.GenerateStatic<ICollection<string>>();
        var c2 = AutoFaker.GenerateStatic<IDictionary<string, string>>();
        var c3 = AutoFaker.GenerateStatic<IEnumerable<string>>();
        var c4 = AutoFaker.GenerateStatic<IList<string>>();
        var c5 = AutoFaker.GenerateStatic<IReadOnlyCollection<string>>();
        var c6 = AutoFaker.GenerateStatic<IReadOnlyDictionary<string, string>>();
        var c7 = AutoFaker.GenerateStatic<IReadOnlyList<string>>();
        var c8 = AutoFaker.GenerateStatic<ISet<string>>();

        c1.Should().NotBeEmpty();
        c2.Should().NotBeEmpty();
        c3.Should().NotBeEmpty();
        c4.Should().NotBeEmpty();
        c5.Should().NotBeEmpty();
        c6.Should().NotBeEmpty();
        c7.Should().NotBeEmpty();
        c8.Should().NotBeEmpty();
    }

    [Fact]
    public void Should_Generate_Collection_Properties()
    {
        var collections = AutoFaker.GenerateStatic<Collections>();

        collections.C1.Should().NotBeEmpty();
        collections.C2.Should().NotBeEmpty();
        collections.C3.Should().NotBeEmpty();
        collections.C4.Should().NotBeEmpty();
        collections.C5.Should().NotBeEmpty();
        collections.C6.Should().NotBeEmpty();
        collections.C7.Should().NotBeEmpty();
        collections.C8.Should().NotBeEmpty();
    }

    //[Fact]
    //public void Should_Generate_Collection_Properties_With_Rules()
    //{
    //    Faker<Collections>? faker = new AutoFaker<Collections>()
    //        .RuleFor(c => c.C1, f => new List<string> { f.Random.Word() })
    //        .RuleFor(c => c.C2, f => new Dictionary<string, string> { { f.Random.Word(), f.Random.Word() } })
    //        .RuleFor(c => c.C3, f => new List<string> { f.Random.Word() })
    //        .RuleFor(c => c.C4, f => new List<string> { f.Random.Word() })
    //        .RuleFor(c => c.C5, f => new List<string> { f.Random.Word() })
    //        .RuleFor(c => c.C6, f => new Dictionary<string, string> { { f.Random.Word(), f.Random.Word() } })
    //        .RuleFor(c => c.C7, f => new List<string> { f.Random.Word() })
    //        .RuleFor(c => c.C8, f => new HashSet<string> { f.Random.Word() });

    //    Collections? collections = faker.Generate();

    //    collections.C1.Should().NotBeEmpty();
    //    collections.C2.Should().NotBeEmpty();
    //    collections.C3.Should().NotBeEmpty();
    //    collections.C4.Should().NotBeEmpty();
    //    collections.C5.Should().NotBeEmpty();
    //    collections.C6.Should().NotBeEmpty();
    //    collections.C7.Should().NotBeEmpty();
    //    collections.C8.Should().NotBeEmpty();

    //}
}