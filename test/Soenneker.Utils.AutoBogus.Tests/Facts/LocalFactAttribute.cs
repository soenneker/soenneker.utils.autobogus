using Soenneker.Utils.AutoBogus.Tests.Utils;
using Xunit;

namespace Soenneker.Utils.AutoBogus.Tests.Facts;

/// <summary>
/// An xUnit Fact attribute derivation that when used to decorate a method it skips the test if used within a pipeline <para/>
/// Replace with [Fact] if you wish to run the test in all environments
/// </summary>
internal class LocalFactAttribute : FactAttribute
{
    private const string _defaultSkip = "LocalFact";

    public string? Reason { get; set; }

    public override string? Skip
    {
        get
        {
            if (!EnvironmentUtil.IsPipeline)
                return null;

            if (Reason != null)
                return $"{_defaultSkip}:{Reason}";

            return _defaultSkip;
        }
    }
}