using System;

namespace Soenneker.Utils.AutoBogus.Services;

internal static class AutoFakerBinderService
{
    internal static AutoFakerBinder Binder => _lazyBinder.Value;

    internal static readonly Lazy<AutoFakerBinder> _lazyBinder = new(() =>
    {
        if (_customBinder != null)
            return _customBinder;

        return new AutoFakerBinder();
    }, true);

    private static AutoFakerBinder? _customBinder;

    internal static void SetCustomBinder(AutoFakerBinder binder)
    {
        _customBinder = binder;
    }
}