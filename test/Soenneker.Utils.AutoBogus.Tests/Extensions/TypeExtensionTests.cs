using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Soenneker.Utils.AutoBogus.Enums;
using Soenneker.Utils.AutoBogus.Extensions;
using Soenneker.Utils.AutoBogus.Tests.Dtos;
using Soenneker.Utils.AutoBogus.Util;
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
    public void IsReadOnlyDictionary_should_be_true()
    {
        Type derivedType = typeof(DerivedReadOnlyDictionary);
        List<Type> interfaces = derivedType.GetInterfaces().ToList(); ;

        (Type, GenericCollectionType) result = interfaces.GetTypeOfGenericCollectionFromInterfaceTypes();

    }

    [Fact]
    public void IsReadOnlyDictionary_should_be_false()
    {
        Type derivedType = typeof(DerivedDictionary);

        bool result = derivedType.IsReadOnlyDictionary();
        result.Should().BeFalse();
    }

    [Fact]
    public void Interfaces()
    {
        Type derivedType = typeof(DerivedDictionary);

        Type[] result = derivedType.GetInterfaces();
    }
}