using System;
using System.Collections.Generic;
using AwesomeAssertions;
using Soenneker.Utils.AutoBogus.Tests.Dtos.Complex;
using System.Diagnostics;
using System.IO;
using Soenneker.Utils.AutoBogus.Tests.Dtos.Simple;
using Xunit;
using System.Linq;
using Bogus;
using Soenneker.Facts.Local;
using Soenneker.Utils.AutoBogus.Config;
using System.Reflection;
using Soenneker.Reflection.Cache.Options;

namespace Soenneker.Utils.AutoBogus.Tests;

public class AutoFakerTTests
{
    [Fact]
    public void Generate_order_should_generate()
    {
        var autoFaker = new AutoFaker<Order>();
        autoFaker.RuleFor(x => x.Id, () => 1);

        Order order = autoFaker.Generate();
        order.Should().NotBeNull();
        order.Id.Should().Be(1);
    }

    [Fact]
    public void Generate_TestClassEmpty_should_generate()
    {
        var faker = new AutoFaker();

        var record = faker.Generate<TestClassEmpty>();
        record.Should().NotBeNull();
    }

    [Fact]
    public void Generate_order_with_count_should_generate()
    {
        var autoFaker = new AutoFaker<Order>();
        autoFaker.RuleFor(x => x.Id, () => 1);

        List<Order> orders = autoFaker.Generate(3);
        orders.Should().NotBeNullOrEmpty();
        orders.Count.Should().Be(3);
        orders.All(x => x.Id == 1).Should().BeTrue();
    }

    //[Fact]
    //public void Generate_order_inline_with_count_should_generate()
    //{
    //    List<Order>? orders = new AutoFaker<Order>()
    //        .RuleFor(e => e.Id, 1)
    //        .RuleFor(e => e.DateCreated, DateTime.UtcNow)
    //        .Generate(3);

    //    orders.Should().NotBeNullOrEmpty();
    //    orders.Count.Should().Be(3);
    //    orders.All(x => x.Id == 1).Should().BeTrue();
    //}

    [Fact]
    public void Generate_record_should_generate()
    {
        var faker = new AutoFaker<TestRecord>();

        TestRecord record = faker.Generate();

        record.Should().NotBeNull();
        record.Name.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Generate_Product_should_generate()
    {
        var faker = new AutoFaker<Product>();

        Product product = faker.Generate();
        product.Should().NotBeNull();
        product.GetRevisions.Should().NotBeNullOrEmpty();
        product.ReadOnlySet.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Generate_many_Orders_should_generate()
    {
        var faker = new AutoFaker<Order>();

        var stopwatch = Stopwatch.StartNew();

        var orders = new List<Order>();

        for (var i = 0; i < 1000; i++)
        {
            orders.Add(faker.Generate());
        }

        orders.Count.Should().Be(1000);

        stopwatch.Stop();
    }

    [Fact]
    public void Generate_with_default_RepeatCount_should_generate_correct_count()
    {
        var autoFaker = new AutoFaker<CustomOrder>();

        CustomOrder order = autoFaker.Generate();
        order.Items.Count().Should().Be(1);
    }

    [Fact]
    public void Generate_with_set_RepeatCount_should_generate_correct_count()
    {
        var autoFaker = new AutoFaker<CustomOrder>
        {
            Config =
            {
                RepeatCount = 3
            }
        };

        CustomOrder order = autoFaker.Generate();
        order.Items.Count().Should().Be(3);
    }

    [Fact]
    public void Generate_budget_and_budget_entries_should_generate()
    {
        Budget? budget = new AutoFaker<Budget>()
            .RuleFor(c => c.IsActive, true)
            .Ignore(c => c.BudgetEntries)
            .Generate();

        budget.Should().NotBeNull();

        List<BudgetEntry>? budgetEntries = new AutoFaker<BudgetEntry>()
            .RuleFor(c => c.Budget, c => budget)
            .Ignore(c => c.Currency)
            .Generate(10);

        budgetEntries.ForEach(c => c.Should().NotBeNull());
    }


    [LocalFact]
    public void TestPrivateReadOnlyFieldWithCtor_Should_Be_Set_via_AutoFaker()
    {
        var config = new AutoFakerConfig
        {
            ReflectionCacheOptions = new ReflectionCacheOptions
            {
                FieldFlags = BindingFlags.Public
            }
        };

        Faker<TestClassPrivateReadOnlyFieldWithCtor> objectToFake = new AutoFaker<TestClassPrivateReadOnlyFieldWithCtor>(config);

        TestClassPrivateReadOnlyFieldWithCtor? obj = objectToFake.Generate();

        obj.GetKey().Should().NotBeNull();
    }

    [LocalFact]
    public void TestPrivateReadOnlyField_Should_Not_be_set_by_AutoFaker()
    {
        var config = new AutoFakerConfig
        {
            ReflectionCacheOptions = new ReflectionCacheOptions
            {
                FieldFlags = BindingFlags.Public
            }
        };

        Faker<TestClassPrivateReadOnlyField> objectToFake = new AutoFaker<TestClassPrivateReadOnlyField>(config);

        TestClassPrivateReadOnlyField? obj = objectToFake.Generate();

        obj.GetKey().Should().BeNull();
    }

    [Fact]
    public void TestWrappedReadOnlyCollectionProperty_Should_not_be_set_by_AutoFaker()
    {
        var config = new AutoFakerConfig
        {
            ReflectionCacheOptions = new ReflectionCacheOptions
            {
                FieldFlags = BindingFlags.Public
            }
        };

        var objectToFake = new AutoFaker<TestClassICollectionPropertyWrappedWithReadOnly>(config);

        TextWriter originalOut = Console.Out;
        try
        {
            using var writer = new StringWriter();
            Console.SetOut(writer);
            TestClassICollectionPropertyWrappedWithReadOnly obj = objectToFake.Generate();

            writer.ToString().Should().BeEmpty();
            obj.Collection.Should().NotBeEmpty();
            obj.WrappedCollection.Should().BeEmpty();
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    [Fact]
    public void TestWrappedReadOnlyDictionaryProperty_Should_not_be_set_by_AutoFaker()
    {
        var config = new AutoFakerConfig
        {
            ReflectionCacheOptions = new ReflectionCacheOptions
            {
                FieldFlags = BindingFlags.Public
            }
        };

        var objectToFake = new AutoFaker<TestClassIDictionaryPropertyWrappedWithReadOnly>(config);

        TextWriter originalOut = Console.Out;
        try
        {
            using var writer = new StringWriter();
            Console.SetOut(writer);
            TestClassIDictionaryPropertyWrappedWithReadOnly obj = objectToFake.Generate();

            writer.ToString().Should().BeEmpty();
            obj.Dictionary.Should().NotBeEmpty();
            obj.WrappedDictionary.Should().BeEmpty();
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    [Fact]
    public void TestClassWithFuncCtor_should_be_null()
    {
        var autoFaker = new AutoFaker<TestClassWithFuncCtor<int>>();
        TestClassWithFuncCtor<int> obj = autoFaker.Generate();
        obj.Should().BeNull();
    }

    [Fact]
    public void TestClass_with_rule_should_be_empty()
    {
        Faker<TestClassWithListString>? autoFaker = new AutoFaker<TestClassWithListString>().RuleFor(c => c.Value, new List<string>());
        TestClassWithListString? result = autoFaker.Generate();

        result.Value.Should().BeEmpty();
    }

    [Fact]
    public void TestClassWithCollectionBackedByReadOnlyCollection_with_rule_should_be_empty()
    {
        var config = new AutoFakerConfig
        {
            RecursiveDepth = 0
        };

        Faker<TestClassWithCollectionBackedByReadOnlyCollection<string>>? autoFaker = new AutoFaker<TestClassWithCollectionBackedByReadOnlyCollection<string>>(config).RuleFor(c => c.PublicList, new List<string>());
        TestClassWithCollectionBackedByReadOnlyCollection<string>? result = autoFaker.Generate();

        result.PublicList.Should().BeEmpty();
    }
}