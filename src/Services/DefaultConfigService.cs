using Soenneker.Utils.AutoBogus.Config;
using System;

namespace Soenneker.Utils.AutoBogus.Services;

internal static class DefaultConfigService
{
    internal static AutoFakerConfig Config => _lazyDefaultConfig.Value;

    internal static readonly Lazy<AutoFakerConfig> _lazyDefaultConfig = new(() => new AutoFakerConfig(), true);
}