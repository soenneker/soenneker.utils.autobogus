using FluentAssertions;
using FluentAssertions.Primitives;
using Soenneker.Utils.AutoBogus.Tests.Dtos;

namespace Soenneker.Utils.AutoBogus.Tests.Extensions;

public static class ObjectAssertionsExtensions
{
    public static AndConstraint<object> BeGenerated(this ObjectAssertions assertions)
    {
        var should = new GenerateAssertions(assertions.Subject);
        return should.BeGenerated();
    }

    public static AndConstraint<object> BeGeneratedWithMocks(this ObjectAssertions assertions)
    {
        var should = new GenerateAssertions(assertions.Subject);
        return should.BeGeneratedWithMocks();
    }
    
    public static AndConstraint<object> BeGeneratedWithoutMocks(this ObjectAssertions assertions)
    {
        var should = new GenerateAssertions(assertions.Subject);
        return should.BeGeneratedWithoutMocks();
    }

    public static AndConstraint<object> NotBeGenerated(this ObjectAssertions assertions)
    {
        var should = new GenerateAssertions(assertions.Subject);
        return should.NotBeGenerated();
    }
}