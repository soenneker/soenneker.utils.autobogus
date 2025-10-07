using System;

namespace Soenneker.Utils.AutoBogus.Tests.Dtos;

/// <summary>
/// Test class with various property types to verify SkipPaths behavior
/// </summary>
internal sealed class TestSkipClass
{
    public string StringProperty { get; set; }
    public int IntProperty { get; set; }
    public DateTime DateTimeProperty { get; set; }
    public bool BoolProperty { get; set; }
    public decimal DecimalProperty { get; set; }
}