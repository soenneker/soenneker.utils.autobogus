using System;
using Soenneker.Utils.AutoBogus.Context;

namespace Soenneker.Utils.AutoBogus.Generators;

internal sealed class AutoFakerGeneratorMemberOverride<TType, TValue> : AutoFakerGeneratorOverride
{
    internal AutoFakerGeneratorMemberOverride(string memberName, Func<AutoFakerOverrideContext, TValue> generator)
    {
        if (string.IsNullOrWhiteSpace(memberName))
        {
            throw new ArgumentException("Value cannot be null or white space", nameof(memberName));
        }

        Type = typeof(TType);
        MemberName = memberName;
        Generator = generator ?? throw new ArgumentNullException(nameof(generator));
    }

    private Type Type { get; }
    private string MemberName { get; }
    private Func<AutoFakerOverrideContext, TValue> Generator { get; }

    public override bool CanOverride(AutoFakerContext context)
    {
        return context.ParentType == Type && MemberName.Equals(context.GenerateName, StringComparison.OrdinalIgnoreCase);
    }

    public override void Generate(AutoFakerOverrideContext context)
    {
        context.Instance = Generator.Invoke(context);
    }
}