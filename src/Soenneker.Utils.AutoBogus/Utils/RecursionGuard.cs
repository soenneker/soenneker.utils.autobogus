using Soenneker.Utils.AutoBogus.Context;

namespace Soenneker.Utils.AutoBogus.Utils;

/// <summary>
/// A guard class that prevents infinite recursion during instance creation.
/// </summary>
public class RecursionGuard
{
    /// <summary>
    /// Gets a value indicating whether the current generation request is recursive.
    /// </summary>
    public bool IsRecursive { get; }

    public RecursionGuard(AutoFakerContext context, int cacheKey)
    {
        foreach (int item in context.RecursiveConstructorStack)
        {
            if (item != cacheKey)
                continue;
            
            IsRecursive = true;
            return;
        }

        // No recursion; push the cache key onto the stack
        context.RecursiveConstructorStack.Push(cacheKey);
        IsRecursive = false;
    }
}