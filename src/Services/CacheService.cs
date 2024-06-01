using System;
using Soenneker.Reflection.Cache;
using Soenneker.Reflection.Cache.Options;

namespace Soenneker.Utils.AutoBogus.Services;

public sealed class CacheService
{
    internal ReflectionCache Cache => _cacheLazy.Value;

    private Lazy<ReflectionCache> _cacheLazy;

    private readonly ReflectionCacheOptions? _reflectionCacheOptions;

    public CacheService(ReflectionCacheOptions? reflectionCacheOptions = null)
    {
        _reflectionCacheOptions = reflectionCacheOptions;
        _cacheLazy = new Lazy<ReflectionCache>(() => new ReflectionCache(_reflectionCacheOptions, true), true);
    }

    internal void ClearCache()
    {
        _cacheLazy = new Lazy<ReflectionCache>(() => new ReflectionCache(_reflectionCacheOptions, true), true);
    }
}