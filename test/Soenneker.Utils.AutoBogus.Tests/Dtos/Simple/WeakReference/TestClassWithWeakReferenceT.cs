using System;

namespace Soenneker.Utils.AutoBogus.Tests.Dtos.Simple.WeakReference;

public class TestClassWithWeakReferenceT
{
    internal WeakReference<TestClassWithSimpleProperties> WeakReference { get; set; }
}