using System;
using Soenneker.Reflection.Cache;

namespace Soenneker.Utils.AutoBogus.Services;

public class CacheService
{
    internal ReflectionCache Cache => _cacheLazy.Value;

    private Lazy<ReflectionCache> _cacheLazy = new(() => new ReflectionCache(), true);

    internal void ClearCache()
    {
        _cacheLazy = new Lazy<ReflectionCache>(() => new ReflectionCache(), true);
    }
}