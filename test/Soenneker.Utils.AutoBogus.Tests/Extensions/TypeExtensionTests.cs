using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Soenneker.Reflection.Cache.Types;
using Soenneker.Utils.AutoBogus.Enums;
using Soenneker.Utils.AutoBogus.Extensions;
using Soenneker.Utils.AutoBogus.Services;
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
        CachedType derivedType = CacheService.Cache.GetCachedType(typeof(DerivedReadOnlyDictionary));
        List<CachedType> interfaces = derivedType.GetCachedInterfaces().ToList();

        (CachedType?, GenericCollectionType?) result = interfaces.GetTypeOfGenericCollectionFromInterfaceTypes();
        result.Item1.Type.Should().Be(typeof(IDictionary<string, object>));
        result.Item2.Should().Be(GenericCollectionType.Dictionary);
    }

    [Fact]
    public void GetTypeOfGenericCollectionFromInterfaceTypes_should_return_dictionary_for_Derived()
    {
        CachedType derivedType = CacheService.Cache.GetCachedType(typeof(DerivedDictionary));
        List<CachedType> interfaces = derivedType.GetCachedInterfaces().ToList();

        (CachedType?, GenericCollectionType?) result = interfaces.GetTypeOfGenericCollectionFromInterfaceTypes();
        result.Item1.Type.Should().Be(typeof(IDictionary<string,object>));
        result.Item2.Should().Be(GenericCollectionType.Dictionary);
    }

    [Fact]
    public void GetTypeOfGenericCollectionFromInterfaceTypes_should_return_dictionary_for_DoubleDerived()
    {
        CachedType derivedType = CacheService.Cache.GetCachedType(typeof(DoubleDerivedDictionary));

        List <CachedType> interfaces = derivedType.GetCachedInterfaces().ToList();

        (CachedType?, GenericCollectionType?) result = interfaces.GetTypeOfGenericCollectionFromInterfaceTypes();
        result.Item1.Type.Should().Be(typeof(IDictionary<string, object>));
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