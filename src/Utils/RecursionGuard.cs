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

    /// <summary>
    /// Initializes a new instance of the <see cref="RecursionGuard"/> class.
    /// </summary>
    /// <param name="context">The <see cref="AutoFakerContext"/> instance for the generate request.</param>
    /// <param name="cacheKey">The cache key of the type being generated.</param>
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