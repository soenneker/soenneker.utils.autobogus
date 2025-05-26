using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AwesomeAssertions;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Generators;
using Soenneker.Utils.AutoBogus.Generators.Abstract;
using Soenneker.Utils.AutoBogus.Generators.Types;
using Soenneker.Utils.AutoBogus.Tests.Dtos.Simple;
using Soenneker.Utils.AutoBogus.Tests.Dtos.Simple.Abstract;
using Xunit;

namespace Soenneker.Utils.AutoBogus.Tests;

public partial class AutoGeneratorsFixture
{    
    public class ReadOnlyDictionaryGenerator
        : AutoGeneratorsFixture
    {
        [Theory]
        [InlineData(typeof(IReadOnlyDictionary<int, string>))]
        [InlineData(typeof(ReadOnlyDictionary<int, string>))]
        public void Generate_Should_Return_Dictionary(Type type)
        {
            Type[] genericTypes = type.GetGenericArguments();
            Type keyType = genericTypes.ElementAt(0);
            Type valueType = genericTypes.ElementAt(1);
            IAutoFakerGenerator generator = CreateGenerator(typeof(ReadOnlyDictionaryGenerator<,>), keyType, valueType);
            var dictionary = InvokeGenerator(type, generator) as IReadOnlyDictionary<int, string>;

            dictionary.Should().NotBeNull().And.NotContainNulls();

            foreach (int key in dictionary.Keys)
            {
                string value = dictionary[key];

                key.Should().BeOfType(keyType);
                value.Should().BeOfType(valueType);
            }
        }

        [Theory]
        [InlineData(typeof(IReadOnlyDictionary<int, TestEnum>))]
        [InlineData(typeof(IReadOnlyDictionary<int, TestStruct>))]
        [InlineData(typeof(IReadOnlyDictionary<int, TestSealedClass>))]
        [InlineData(typeof(IReadOnlyDictionary<int, ITestInterface>))]
        [InlineData(typeof(IReadOnlyDictionary<int, TestAbstractClass>))]
        [InlineData(typeof(ReadOnlyDictionary<int, TestSealedClass>))]
        public void GetGenerator_Should_Return_ReadOnlyDictionaryGenerator(Type type)
        {
            AutoFakerContext context = CreateContext(type);
            Type[] genericTypes = type.GetGenericArguments();
            Type keyType = genericTypes.ElementAt(0);
            Type valueType = genericTypes.ElementAt(1);
            Type generatorType = GetGeneratorType(typeof(ReadOnlyDictionaryGenerator<,>), keyType, valueType);

            AutoFakerGeneratorFactory.GetGenerator(context).Should().BeOfType(generatorType);
        }
    }
}