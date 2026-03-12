using System;
using System.Collections.Generic;
using Soenneker.Reflection.Cache.Types;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Generators.Abstract;

namespace Soenneker.Utils.AutoBogus.Generators.Types;

internal sealed class ExpandoObjectGenerator : IAutoFakerGenerator
{
    object IAutoFakerGenerator.Generate(AutoFakerContext context)
    {
        object instance = context.Instance;

        // Need to copy the target dictionary to avoid mutations during population
        var target = instance as IDictionary<string, object>;
        var source = new Dictionary<string, object>(target);

        List<KeyValuePair<string, object>> properties = [];

        foreach (KeyValuePair<string, object> pair in source)
        {
            if (pair.Value != null)
            {
                properties.Add(pair);
            }
        }

        foreach (KeyValuePair<string, object> property in properties)
        {
            // Configure the context
            Type type = property.Value.GetType();

            context.Setup(context.CachedType, context.CacheService.Cache.GetCachedType(type), property.Key);

            CachedType cachedType = context.CacheService.Cache.GetCachedType(type);

            if (cachedType.IsExpandoObject)
            {
                context.Instance = property.Value;
            }
            else
            {
                context.Instance = null;
            }

            // Generate the property values
            IAutoFakerGenerator generator = AutoFakerGeneratorFactory.GetGenerator(context);
            target[property.Key] = generator.Generate(context);
        }

        // Reset the instance context
        context.Instance = instance;

        return instance;
    }
}