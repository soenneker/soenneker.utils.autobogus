using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Soenneker.Utils.AutoBogus.Enums;
using Soenneker.Utils.AutoBogus.Extensions;
using Soenneker.Utils.AutoBogus.Tests.Dtos;
using Xunit;

namespace Soenneker.Utils.AutoBogus.Tests.Extensions;

public class TypeExtensionTests
{
    [Fact]
    public void IsDictionary_should_be_true()
    {
        Type derivedType = typeof(DerivedDictionary);

        bool result = derivedType.IsDictionary();
        result.Should().BeTrue();
    }

    [Fact]
    public void GetTypeOfGenericCollectionFromInterfaceTypes_should_return_readonly_dictionary()
    {
        Type derivedType = typeof(DerivedReadOnlyDictionary);
        List<Type> interfaces = derivedType.GetInterfaces().ToList();

        (Type?, GenericCollectionType?) result = interfaces.GetTypeOfGenericCollectionFromInterfaceTypes();
        result.Item1.Should().Be(typeof(IDictionary<string, object>));
        result.Item2.Should().Be(GenericCollectionType.Dictionary);
    }

    [Fact]
    public void GetTypeOfGenericCollectionFromInterfaceTypes_should_return_dictionary_for_Derived()
    {
        Type derivedType = typeof(DerivedDictionary);
        List<Type> interfaces = derivedType.GetInterfaces().ToList();

        (Type?, GenericCollectionType?) result = interfaces.GetTypeOfGenericCollectionFromInterfaceTypes();
        result.Item1.Should().Be(typeof(IDictionary<string,object>));
        result.Item2.Should().Be(GenericCollectionType.Dictionary);
    }

    [Fact]
    public void GetTypeOfGenericCollectionFromInterfaceTypes_should_return_dictionary_for_DoubleDerived()
    {
        Type derivedType = typeof(DoubleDerivedDictionary);
        List<Type> interfaces = derivedType.GetInterfaces().ToList();

        (Type?, GenericCollectionType?) result = interfaces.GetTypeOfGenericCollectionFromInterfaceTypes();
        result.Item1.Should().Be(typeof(IDictionary<string, object>));
        result.Item2.Should().Be(GenericCollectionType.Dictionary);
    }

    [Fact]
    public void IsReadOnlyDictionary_should_be_true()
    {
        Type derivedType = typeof(DerivedReadOnlyDictionary);

        bool result = derivedType.IsReadOnlyDictionary();
        result.Should().BeTrue();
    }
}