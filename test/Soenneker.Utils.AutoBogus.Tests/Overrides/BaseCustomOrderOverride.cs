using System.Collections.Generic;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Override;
using Soenneker.Utils.AutoBogus.Tests.Dtos.Complex;

namespace Soenneker.Utils.AutoBogus.Tests.Overrides;

public class BaseCustomOrderOverride : AutoFakerOverride<BaseCustomOrder>
{
    public override void Generate(AutoFakerOverrideContext context)
    {
        var target = (context.Instance as BaseCustomOrder)!;

    }
}