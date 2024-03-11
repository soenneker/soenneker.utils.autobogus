using System;
using System.Collections.Generic;
using System.Linq;
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
        IEnumerable<KeyValuePair<string, object>> properties = source.Where(pair => pair.Value != null);

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