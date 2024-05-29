using System;

namespace Soenneker.Utils.AutoBogus.Tests.Dtos.Simple;

internal class TestClassWithFuncCtor<T>
{
    private Func<T> _func;

    public TestClassWithFuncCtor(Func<T> func)
    {
        _func = func;
    }
}