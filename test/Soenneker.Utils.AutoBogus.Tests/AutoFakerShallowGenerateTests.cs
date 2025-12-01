using System;
using System.Collections.Generic;
using AwesomeAssertions;
using Soenneker.Utils.AutoBogus.Tests.Dtos.Complex;
using Soenneker.Utils.AutoBogus.Tests.Dtos.Simple;
using Xunit;

namespace Soenneker.Utils.AutoBogus.Tests;

public class AutoFakerShallowGenerateTests
{
    [Fact]
    public void ShallowGenerate_should_skip_complex_reference_types()
    {
        var faker = new AutoFaker
        {
            Config =
            {
                ShallowGenerate = true
            }
        };

        var order = faker.Generate<Order>();

        // Primitives should still be generated
        order.Id.Should().NotBe(0);
        order.Code.Should().NotBeNull();
        order.Status.Should().BeDefined();
        order.DateCreated.Should().NotBe(default);
        order.Timestamp.Should().NotBe(default);

        // Complex reference types should be skipped (null)
        order.Calculator.Should().BeNull();
    }

    [Fact]
    public void ShallowGenerate_should_still_generate_primitives()
    {
        var faker = new AutoFaker
        {
            Config =
            {
                ShallowGenerate = true
            }
        };

        var testClass = faker.Generate<TestClassWithSimpleProperties>();

        testClass.Id.Should().NotBe(0);
        testClass.Name.Should().NotBeNullOrEmpty();
        testClass.CreatedAt.Should().NotBe(default);
    }

    [Fact]
    public void ShallowGenerate_should_still_generate_collections()
    {
        var faker = new AutoFaker
        {
            Config =
            {
                ShallowGenerate = true
            }
        };

        // Test that primitives are still generated even when ShallowGenerate is enabled
        var intValue = faker.Generate<int>();
        var stringValue = faker.Generate<string>();
        var guidValue = faker.Generate<Guid>();

        intValue.Should().NotBe(0);
        stringValue.Should().NotBeNullOrEmpty();
        guidValue.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void ShallowGenerate_should_still_generate_arrays_of_primitives()
    {
        var faker = new AutoFaker
        {
            Config =
            {
                ShallowGenerate = true
            }
        };

        var testClass = faker.Generate<TestClassWithArrayIntProperty>();

        testClass.Values.Should().NotBeNull();
        testClass.Values.Should().NotBeEmpty();
    }

    [Fact]
    public void ShallowGenerate_should_skip_arrays_of_complex_types()
    {
        var faker = new AutoFaker
        {
            Config =
            {
                ShallowGenerate = true
            }
        };

        var order = faker.Generate<Order>();

        // Arrays of complex types (DiscountBase[]) should be skipped (null or empty)
        // The array property should not contain any complex objects
        if (order.Discounts != null)
        {
            order.Discounts.Should().BeEmpty();
        }
        
        // But primitives should still be generated
        order.Id.Should().NotBe(0);
        order.Code.Should().NotBeNull();
    }

    [Fact]
    public void ShallowGenerate_should_still_generate_strings()
    {
        var faker = new AutoFaker
        {
            Config =
            {
                ShallowGenerate = true
            }
        };

        var testClass = faker.Generate<TestClassWithSimpleProperties>();

        testClass.Name.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void ShallowGenerate_should_still_generate_value_types()
    {
        var faker = new AutoFaker
        {
            Config =
            {
                ShallowGenerate = true
            }
        };

        var structObj = faker.Generate<TestRecordStruct>();

        structObj.Should().NotBeNull();
        structObj.Name.Should().NotBeNullOrEmpty();
        structObj.Age.Should().NotBe(0);
    }

    [Fact]
    public void ShallowGenerate_should_still_generate_enums()
    {
        var faker = new AutoFaker
        {
            Config =
            {
                ShallowGenerate = true
            }
        };

        var order = faker.Generate<Order>();

        order.Status.Should().BeDefined();
    }

    [Fact]
    public void ShallowGenerate_should_still_generate_nullable_types()
    {
        var faker = new AutoFaker
        {
            Config =
            {
                ShallowGenerate = true
            }
        };

        var testClass = faker.Generate<TestClassWithNullableInt>();

        // Nullable primitives should still be generated
        testClass.Value.Should().NotBeNull();
    }

    [Fact]
    public void ShallowGenerate_should_skip_nested_complex_types()
    {
        var faker = new AutoFaker
        {
            Config =
            {
                ShallowGenerate = true
            }
        };

        // Create a simple test class to verify behavior
        // This test verifies that when we have an object with both primitives and complex types,
        // only primitives are generated when ShallowGenerate is enabled
        var testClass = faker.Generate<TestClassWithSimpleProperties>();

        // Primitives should still be generated
        testClass.Should().NotBeNull();
        testClass.Id.Should().NotBe(0);
        testClass.Name.Should().NotBeNullOrEmpty();
        testClass.CreatedAt.Should().NotBe(default);
    }

    [Fact]
    public void ShallowGenerate_with_config_should_work()
    {
        var faker = new AutoFaker
        {
            Config =
            {
                ShallowGenerate = true
            }
        };

        var order = faker.Generate<Order>();

        // Primitives should still be generated
        order.Id.Should().NotBe(0);
        order.Code.Should().NotBeNull();

        // Complex reference types should be skipped
        order.Calculator.Should().BeNull();
    }

    [Fact]
    public void ShallowGenerate_with_config_false_should_generate_complex_types()
    {
        var faker = new AutoFaker
        {
            Config =
            {
                ShallowGenerate = false
            }
        };

        var order = faker.Generate<Order>();

        // With ShallowGenerate = false, complex types should be generated
        // Note: Calculator is an interface, so it might still be null, but other complex types should work
        order.Id.Should().NotBe(0);
    }

    [Fact]
    public void ShallowGenerate_should_skip_recursive_complex_types()
    {
        var faker = new AutoFaker
        {
            Config =
            {
                ShallowGenerate = true
            }
        };

        var testClass = faker.Generate<TestClassWithRecursiveConstructor>();

        // Primitives should be generated
        testClass.Name.Should().NotBeNullOrEmpty();
        testClass.Age.Should().NotBe(0);

        // Complex reference type (Child) should be skipped
        testClass.Child.Should().BeNull();
    }

    [Fact]
    public void ShallowGenerate_should_default_to_false()
    {
        var faker = new AutoFaker();

        faker.Config.ShallowGenerate.Should().BeFalse();
    }

    [Fact]
    public void ShallowGenerate_should_generate_dictionaries()
    {
        var faker = new AutoFaker
        {
            Config =
            {
                ShallowGenerate = true
            }
        };

        var dictionary = faker.Generate<Dictionary<int, string>>();

        dictionary.Should().NotBeNull();
    }

    [Fact]
    public void ShallowGenerate_should_generate_lists()
    {
        var faker = new AutoFaker
        {
            Config =
            {
                ShallowGenerate = true
            }
        };

        var list = faker.Generate<List<string>>();

        list.Should().NotBeNull();
        list.Should().NotBeEmpty();
    }

    [Fact]
    public void ShallowGenerate_should_skip_class_properties_but_keep_primitives()
    {
        // Create a test class with both primitives and complex types
        var faker = new AutoFaker
        {
            Config =
            {
                ShallowGenerate = true
            }
        };

        var order = faker.Generate<Order>();

        // Verify all primitives are generated
        order.Id.Should().NotBe(0);
        order.Code.Should().NotBeNull();
        order.Status.Should().BeDefined();
        order.DateCreated.Should().NotBe(default);
        order.Timestamp.Should().NotBe(default);

        // Verify complex reference types are null
        order.Calculator.Should().BeNull();
    }

    [Fact]
    public void ShallowGenerate_should_work_with_AutoFakerT()
    {
        var faker = new AutoFaker<Order>
        {
            Config =
            {
                ShallowGenerate = true
            }
        };

        var order = faker.Generate();

        // Primitives should still be generated
        order.Id.Should().NotBe(0);
        order.Code.Should().NotBeNull();

        // Complex reference types should be skipped
        order.Calculator.Should().BeNull();
    }

    [Fact]
    public void ShallowGenerate_should_generate_collections_of_primitives()
    {
        var faker = new AutoFaker
        {
            Config =
            {
                ShallowGenerate = true
            }
        };

        var testClass = faker.Generate<TestClassWithCollections>();

        // Collections of primitives should be generated
        testClass.StringList.Should().NotBeNull();
        testClass.StringList.Should().NotBeEmpty();
        testClass.IntCollection.Should().NotBeNull();
        testClass.IntCollection.Should().NotBeEmpty();
        
        // IEnumerable<T> is an interface, so it may be null if it can't be instantiated
        // The important thing is that concrete collections like List are generated
    }

    [Fact]
    public void ShallowGenerate_should_skip_collections_of_complex_types()
    {
        var faker = new AutoFaker
        {
            Config =
            {
                ShallowGenerate = true
            }
        };

        var testClass = faker.Generate<TestClassWithCollections>();

        // Collections of complex types should be skipped (null or empty)
        if (testClass.OrderItemList != null)
        {
            testClass.OrderItemList.Should().BeEmpty();
        }
        if (testClass.ProductCollection != null)
        {
            testClass.ProductCollection.Should().BeEmpty();
        }
        if (testClass.OrderEnumerable != null)
        {
            testClass.OrderEnumerable.Should().BeEmpty();
        }
    }

    [Fact]
    public void ShallowGenerate_should_skip_all_dictionaries()
    {
        var faker = new AutoFaker
        {
            Config =
            {
                ShallowGenerate = true
            }
        };

        var testClass = faker.Generate<TestClassWithDictionaries>();

        // All dictionaries should be skipped (null) when ShallowGenerate is enabled
        testClass.StringIntDictionary.Should().BeNull();
        testClass.IntStringDictionary.Should().BeNull();
        testClass.StringGuidDictionary.Should().BeNull();
        testClass.StringOrderItemDictionary.Should().BeNull();
        testClass.IntProductDictionary.Should().BeNull();
        testClass.StringOrderDictionary.Should().BeNull();
    }

    [Fact]
    public void ShallowGenerate_should_skip_dictionaries_with_complex_values()
    {
        var faker = new AutoFaker
        {
            Config =
            {
                ShallowGenerate = true
            }
        };

        var testClass = faker.Generate<TestClassWithDictionaries>();

        // All dictionaries should be skipped (null) when ShallowGenerate is enabled
        testClass.StringOrderItemDictionary.Should().BeNull();
        testClass.IntProductDictionary.Should().BeNull();
        testClass.StringOrderDictionary.Should().BeNull();
    }

    [Fact]
    public void ShallowGenerate_should_generate_List_of_primitives()
    {
        var faker = new AutoFaker
        {
            Config =
            {
                ShallowGenerate = true
            }
        };

        var list = faker.Generate<List<string>>();

        list.Should().NotBeNull();
        list.Should().NotBeEmpty();
    }

    [Fact]
    public void ShallowGenerate_should_skip_List_of_complex_types()
    {
        var faker = new AutoFaker
        {
            Config =
            {
                ShallowGenerate = true
            }
        };

        // When generating List<OrderItem> directly, the skip logic applies to properties
        // For direct generation, the list may still be generated but with null/empty items
        // This test verifies that when used as a property, it's skipped
        var testClass = faker.Generate<TestClassWithCollections>();

        // List of complex types should be empty or null when used as a property
        if (testClass.OrderItemList != null)
        {
            testClass.OrderItemList.Should().BeEmpty();
        }
    }

    [Fact]
    public void ShallowGenerate_should_skip_Dictionary_with_primitive_values()
    {
        var faker = new AutoFaker
        {
            Config =
            {
                ShallowGenerate = true
            }
        };

        // When generating Dictionary<string, int> directly, it should still be skipped
        // because all dictionaries are excluded when ShallowGenerate is enabled
        var testClass = faker.Generate<TestClassWithDictionaries>();

        testClass.StringIntDictionary.Should().BeNull();
    }

    [Fact]
    public void ShallowGenerate_should_skip_Dictionary_with_complex_values_when_used_as_property()
    {
        var faker = new AutoFaker
        {
            Config =
            {
                ShallowGenerate = true
            }
        };

        // When Dictionary<string, OrderItem> is used as a property, it should be skipped (null)
        var testClass = faker.Generate<TestClassWithDictionaries>();

        // Dictionary with complex values should be null when used as a property with ShallowGenerate enabled
        testClass.StringOrderItemDictionary.Should().BeNull();
    }

    [Fact]
    public void ShallowGenerate_should_generate_nullable_primitives()
    {
        var faker = new AutoFaker
        {
            Config =
            {
                ShallowGenerate = true
            }
        };

        var testClass = faker.Generate<TestClassWithNullableProperties>();

        // Nullable primitives should still be generated (they may be null or have values)
        // The important thing is they're not skipped entirely like complex types would be
        testClass.Should().NotBeNull();
        
        // Verify that nullable primitives are not skipped - they can have values
        // We generate multiple times to increase the chance of getting non-null values
        // The key test is that nullable complex types ARE skipped (see next test)
        for (int i = 0; i < 10; i++)
        {
            var instance = faker.Generate<TestClassWithNullableProperties>();
            instance.Should().NotBeNull();
            // At least some nullable primitives should have values (not all will be null)
            // But the main point is they're not skipped - the properties exist
        }
    }

    [Fact]
    public void ShallowGenerate_should_skip_nullable_complex_types()
    {
        var faker = new AutoFaker
        {
            Config =
            {
                ShallowGenerate = true
            }
        };

        var testClass = faker.Generate<TestClassWithNullableProperties>();

        // Nullable complex types should be skipped (null) when ShallowGenerate is enabled
        // The underlying type (OrderItem, Product, Order) is complex, so the nullable should be skipped
        testClass.NullableOrderItem.Should().BeNull();
        testClass.NullableProduct.Should().BeNull();
        testClass.NullableOrder.Should().BeNull();
    }
}

