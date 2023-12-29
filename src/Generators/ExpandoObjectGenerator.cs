using System;
using System.Collections.Generic;
using System.Linq;
using Soenneker.Utils.AutoBogus.Abstract;
using Soenneker.Utils.AutoBogus.Util;

namespace Soenneker.Utils.AutoBogus.Generators;

internal sealed class ExpandoObjectGenerator : IAutoGenerator
{
    object IAutoGenerator.Generate(AutoGenerateContext context)
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

            context.ParentType = context.GenerateType;
            context.GenerateType = type;
            context.GenerateName = property.Key;

            if (ReflectionHelper.IsExpandoObject(type))
            {
                context.Instance = property.Value;
            }
            else
            {
                context.Instance = null;
            }

            // Generate the property values
            IAutoGenerator generator = AutoGeneratorFactory.GetGenerator(context);
            target[property.Key] = generator.Generate(context);
        }

        // Reset the instance context
        context.Instance = instance;

        return instance;
    }
}