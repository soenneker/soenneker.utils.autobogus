using System;
using System.Collections.Generic;
using Soenneker.Reflection.Cache.Options;
using Soenneker.Utils.AutoBogus.Generators;

namespace Soenneker.Utils.AutoBogus.Config;

public sealed class AutoFakerConfig
{
    /// <summary>
    /// The Bogus.Faker locale to use when generating values.
    /// </summary>
    // ReSharper disable once FieldCanBeMadeReadOnly.Global
    public string Locale;

    /// <summary>
    /// Gets or sets the default number of items to generate when creating collections (lists, arrays, etc.).
    /// </summary>
    /// <remarks>
    /// This value is used when generating collections if no specific count is provided. For example, when generating
    /// a <see cref="List{T}"/>, this many items will be created by default.
    /// </remarks>
    public int RepeatCount;

    /// <summary>
    /// Gets or sets the number of rows to generate when creating <see cref="System.Data.DataTable"/> instances.
    /// </summary>
    /// <remarks>
    /// When generating DataTable objects, this specifies how many data rows should be created. If not set,
    /// a random number of rows will be generated.
    /// </remarks>
    public int DataTableRowCount;

    /// <summary>
    /// Gets or sets the maximum depth for recursively generating the same type within an object graph.
    /// </summary>
    /// <remarks>
    /// This prevents infinite loops when generating types that reference themselves (e.g., a Person with a Parent property of type Person).
    /// When the specified depth is reached for a type, further generation of that type is skipped. A value of 0 disables recursive generation.
    /// </remarks>
    public int RecursiveDepth;

    /// <summary>
    /// Gets or sets the collection of types to skip when generating values. Properties or fields of these types will be left as their default values.
    /// </summary>
    /// <remarks>
    /// This is useful for excluding complex types, expensive-to-generate types, or types that should remain null/default.
    /// Once a type is in this collection, all properties and fields of that type will be skipped during generation.
    /// </remarks>
    public HashSet<Type>? SkipTypes;

    /// <summary>
    /// Gets or sets the collection of member paths to skip when generating values. Paths are in the format "TypeName.MemberName".
    /// </summary>
    /// <remarks>
    /// This allows fine-grained control over which specific members are generated. For example, "Person.Parent" would skip
    /// the Parent property on the Person type. The member will be left as its default value. Useful for excluding computed properties,
    /// read-only fields, or members that should remain uninitialized for testing purposes.
    /// </remarks>
    public HashSet<string>? SkipPaths;

    /// <summary>
    /// Gets or sets the list of custom generator overrides that provide custom logic for generating specific types or members.
    /// </summary>
    /// <remarks>
    /// Overrides allow you to provide custom generation logic for specific types or members. When an override matches
    /// a generation request (based on its <see cref="AutoFakerGeneratorOverride.CanOverride"/> method), it will be used
    /// instead of the default generation logic. Multiple overrides can be registered and will be evaluated in order.
    /// </remarks>
    public List<AutoFakerGeneratorOverride>? Overrides;

    /// <summary>
    /// Gets or sets the maximum depth of the object tree to generate, controlling how many levels of nested objects are created.
    /// </summary>
    /// <remarks>
    /// This limits the overall depth of the object graph, regardless of type. For example, with depth 2:
    /// Root object (depth 0) → Child object (depth 1) → Grandchild object (depth 2, will be skipped).
    /// Use <see langword="null"/> for unlimited depth. This is useful for controlling the size and complexity of generated data structures.
    /// </remarks>
    public int? TreeDepth;

    /// <summary>
    /// Gets or sets the <see cref="DateTimeKind"/> to use when generating DateTime values.
    /// </summary>
    // ReSharper disable once FieldCanBeMadeReadOnly.Global
    public DateTimeKind DateTimeKind;

    /// <summary>
    /// Gets or sets the default timezone offset to use when generating DateTime values.
    /// </summary>
    public TimeSpan? DefaultTimezoneOffset;

    /// <summary>
    /// Gets or sets the reflection cache options to use for type caching.
    /// </summary>
    public ReflectionCacheOptions? ReflectionCacheOptions;

    /// <summary>
    /// Indicates whether generation should be limited to primitive types only, skipping complex objects and collections.
    /// </summary>
    /// <remarks>
    /// When set to <see langword="true"/>, only primitive types (int, string, bool, DateTime, etc.) and value types are generated.
    /// Complex reference types (classes), collections, and dictionaries are skipped and left as their default values (typically <see langword="null"/>).
    /// This is useful for performance optimization when you only need basic data types populated, or when you want to manually handle complex object creation.
    /// </remarks>
    public bool ShallowGenerate;

    /// <summary>
    /// Initializes a new instance of the <see cref="AutoFakerConfig"/> class with default values.
    /// </summary>
    public AutoFakerConfig()
    {
        Locale = AutoFakerDefaultConfigOptions.Locale;
        RepeatCount = AutoFakerDefaultConfigOptions.RepeatCount;
        DataTableRowCount = AutoFakerDefaultConfigOptions.DataTableRowCount;
        RecursiveDepth = AutoFakerDefaultConfigOptions.RecursiveDepth;
        TreeDepth = AutoFakerDefaultConfigOptions.TreeDepth;
        DateTimeKind = AutoFakerDefaultConfigOptions.DateTimeKind;
        DefaultTimezoneOffset = AutoFakerDefaultConfigOptions.DefaultTimezoneOffset;
        ShallowGenerate = AutoFakerDefaultConfigOptions.ShallowGenerate;
    }
}