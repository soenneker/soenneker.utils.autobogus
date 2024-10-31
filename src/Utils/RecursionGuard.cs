using Soenneker.Utils.AutoBogus.Context;

namespace Soenneker.Utils.AutoBogus.Utils;

public class RecursionGuard
{
    public bool IsRecursive { get; }

    public RecursionGuard(AutoFakerContext context, int cacheKey)
    {
        foreach (int item in context.RecursiveConstructorStack)
        {
            if (item == cacheKey)
            {
                IsRecursive = true;
                return;
            }
        }

        // No recursion; push the cache key onto the stack
        context.RecursiveConstructorStack.Push(cacheKey);
        IsRecursive = false;
    }
}