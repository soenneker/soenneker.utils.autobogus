using System.Linq;
using Soenneker.Utils.AutoBogus.Context;

namespace Soenneker.Utils.AutoBogus.Utils;

public class RecursionGuard
{
    public bool IsRecursive { get; }

    public RecursionGuard(AutoFakerContext context, int cacheKey)
    {
        int stackCount = context.RecursiveConstructorStack.Count(c => c == cacheKey);

        if (stackCount >= 1)
        {
            // Recursion detected
            IsRecursive = true;
        }
        else
        {
            // No recursion; push the cache key onto the stack
            context.RecursiveConstructorStack.Push(cacheKey);
            IsRecursive = false;
        }
    }
}