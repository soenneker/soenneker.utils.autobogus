using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace Soenneker.Utils.AutoBogus.Tests.Playground;

public class EntityFrameworkFixture
{
    public class Parent
    {
        public virtual Other Other { get; set; }
        public virtual ICollection<Child> Children { get; set; }
    }

    public class Child
    {
        public virtual Parent Parent { get; set; }
        public virtual ICollection<Other> Items { get; set; }
    }

    public class Other
    {
        public virtual Child Child { get; set; }
        public virtual ICollection<Item> Items { get; set; }
    }

    public class Item
    {
        public virtual Other Other { get; set; }
    }

    [Fact]
    public void TestAutoFaker()
    {
        var parent = AutoFaker.GenerateStatic<Parent>(builder => builder.WithTreeDepth(2));
        parent.Should().NotBeNull();
    }
}