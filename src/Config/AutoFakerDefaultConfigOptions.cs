using System;

namespace Soenneker.Utils.AutoBogus.Config;

public static class AutoFakerDefaultConfigOptions
{
    internal const string Locale = "en";
    internal const int GenerateAttemptsThreshold = 3;

    internal const int RepeatCount = 1;
    internal const int DataTableRowCount = 15;

    internal const int RecursiveDepth = 2;

    internal static readonly int? TreeDepth = null;

    internal static readonly DateTimeKind DateTimeKind = DateTimeKind.Utc;

    internal static readonly TimeSpan? DefaultTimezoneOffset = null;
}