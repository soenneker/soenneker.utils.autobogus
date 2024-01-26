using System;
using System.Diagnostics.Contracts;
using System.Threading;

namespace Soenneker.Utils.AutoBogus.Tests.Utils;

/// <summary>
/// A utility library for useful environment related functionality
/// </summary>
internal static class EnvironmentUtil
{
    // Init needs to be done outside of ctor because Fact evaluates before the ctor of the test
    private static readonly Lazy<bool> _isPipelineLazy = new(() =>
    {
        string? pipelineEnv = Environment.GetEnvironmentVariable("PipelineEnvironment");

        _ = bool.TryParse(pipelineEnv, out bool isPipeline);

        return isPipeline;
    }, LazyThreadSafetyMode.ExecutionAndPublication);

    /// <summary>
    /// Set the Environment variable "PipelineEnvironment" to "true" for this to return true. <para/>
    /// </summary>
    /// <remarks>Syntactic sugar for lazy instance</remarks>
    [Pure]
    public static bool IsPipeline => _isPipelineLazy.Value;

}