using System;
using System.Collections.Generic;
using System.Linq;
using AwesomeAssertions;
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

        var cacheService = new CacheService();
        CachedType cachedType = cacheService.Cache.GetCachedType(derivedType);

        bool result = cachedType.IsDictionary;
        result.Should().BeTrue();
    }

    [Fact]
    public void GetTypeOfGenericCollectionFromInterfaceTypes_should_return_readonly_dictionary()
    {
        var cacheService = new CacheService();
        CachedType derivedType = cacheService.Cache.GetCachedType(typeof(DerivedReadOnlyDictionary));
        List<CachedType> interfaces = derivedType.GetCachedInterfaces().ToList();

        (CachedType?, GenericCollectionType?) result = interfaces.GetTypeOfGenericCollectionFromInterfaceTypes();
        result.Item1.Type.Should().Be(typeof(IDictionary<string, object>));
        result.Item2.Should().Be(GenericCollectionType.Dictionary);
    }

    [Fact]
    public void GetTypeOfGenericCollectionFromInterfaceTypes_should_return_dictionary_for_Derived()
    {
        var cacheService = new CacheService();
        CachedType derivedType = cacheService.Cache.GetCachedType(typeof(DerivedDictionary));
        List<CachedType> interfaces = derivedType.GetCachedInterfaces().ToList();

        (CachedType?, GenericCollectionType?) result = interfaces.GetTypeOfGenericCollectionFromInterfaceTypes();
        result.Item1.Type.Should().Be(typeof(IDictionary<string,object>));
        result.Item2.Should().Be(GenericCollectionType.Dictionary);
    }

    [Fact]
    public void GetTypeOfGenericCollectionFromInterfaceTypes_should_return_dictionary_for_DoubleDerived()
    {
        var cacheService = new CacheService();
        CachedType derivedType = cacheService.Cache.GetCachedType(typeof(DoubleDerivedDictionary));

        List <CachedType> interfaces = derivedType.GetCachedInterfaces().ToList();

        (CachedType?, GenericCollectionType?) result = interfaces.GetTypeOfGenericCollectionFromInterfaceTypes();
        result.Item1.Type.Should().Be(typeof(IDictionary<string, object>));
        result.Item2.Should().Be(GenericCollectionType.Dictionary);
    }

    [Fact]
    public void IsReadOnlyDictionary_should_be_true()
    {
        var cacheService = new CacheService();
        Type derivedType = typeof(DerivedReadOnlyDictionary);
        CachedType cachedType = cacheService.Cache.GetCachedType(derivedType);

        bool result = cachedType.IsReadOnlyDictionary;
        result.Should().BeTrue();
    }
}