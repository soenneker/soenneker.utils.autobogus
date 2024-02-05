namespace Soenneker.Utils.AutoBogus.Services;

internal static class AutoFakerBinderService
{
    internal static AutoFakerBinder Binder { get; private set; } = default!;

    internal static void SetBinder(AutoFakerBinder binder)
    {
        Binder = binder;
    }
}