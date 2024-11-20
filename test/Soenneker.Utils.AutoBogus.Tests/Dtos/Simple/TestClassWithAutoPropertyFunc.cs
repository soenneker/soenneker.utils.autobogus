using System;

namespace Soenneker.Utils.AutoBogus.Tests.Dtos.Simple;

internal class TestClassWithAutoPropertyFunc
{
    public Func<string, string> Func { get; set; }

}