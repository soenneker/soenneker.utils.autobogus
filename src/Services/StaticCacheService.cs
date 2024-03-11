using System;
using Soenneker.Reflection.Cache;

namespace Soenneker.Utils.AutoBogus.Services;

internal static class StaticCacheService
{
    internal static ReflectionCache Cache => _cacheLazy.Value;

    private static readonly Lazy<ReflectionCache> _cacheLazy = new(() => new ReflectionCache(), true);
}