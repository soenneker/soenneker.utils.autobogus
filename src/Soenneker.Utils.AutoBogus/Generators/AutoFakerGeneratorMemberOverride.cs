using System;
using Soenneker.Reflection.Cache.Types;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Services;

namespace Soenneker.Utils.AutoBogus.Generators;

internal sealed class AutoFakerGeneratorMemberOverride<TType, TValue> : AutoFakerGeneratorOverride
{
    private Type Type { get; }

    private readonly CachedType _cachedType;

    private string MemberName { get; }

    private Func<AutoFakerOverrideContext, TValue> Generator { get; }

    internal AutoFakerGeneratorMemberOverride(string memberName, Func<AutoFakerOverrideContext, TValue> generator)
    {
        if (string.IsNullOrWhiteSpace(memberName))
        {
            throw new ArgumentException("Value cannot be null or white space", nameof(memberName));
        }

        Type = typeof(TType);
        _cachedType = StaticCacheService.Cache.GetCachedType(Type);
        MemberName = memberName;
        Generator = generator ?? throw new ArgumentNullException(nameof(generator));
    }

    public override bool CanOverride(AutoFakerContext context)
    {
        return context.ParentType == _cachedType && MemberName.Equals(context.GenerateName, StringComparison.OrdinalIgnoreCase);
    }

    public override void Generate(AutoFakerOverrideContext context)
    {
        context.Instance = Generator.Invoke(context);
    }
}