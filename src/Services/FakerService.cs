namespace Soenneker.Utils.AutoBogus.Services;

//public static class FakerService
//{
//    internal static Faker Faker => _lazyFaker.Value;

//    internal static bool IsValueCreated { get; private set; }
//    internal static bool IsCustomFaker { get; private set; }

//    private static readonly ResettableLazy<Faker> _lazyFaker = new(() =>
//    {
//        IsValueCreated = true;

//        if (_configuredFaker != null)
//            return _configuredFaker;

//        return new Faker();
//    });

//    private static Faker? _configuredFaker;

//    public static void Set(Faker faker)
//    {
//        IsCustomFaker = true;
//        _configuredFaker = faker;
//        _lazyFaker.Reset();
//    }

//    public static void Reset()
//    {
//        IsCustomFaker = false;
//        _configuredFaker = null;
//        _lazyFaker.Reset();
//    }
//}