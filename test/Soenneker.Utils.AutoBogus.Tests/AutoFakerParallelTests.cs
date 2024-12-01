using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Soenneker.Utils.AutoBogus.Tests.Dtos.Simple;
using Xunit;

namespace Soenneker.Utils.AutoBogus.Tests;

public class AutoFakerParallelTests
{
    [Fact]
    public void Generate_with_ParallelExecution()
    {
        // Arrange
        const int numberOfTasks = 1000;
        var results = new ConcurrentBag<TestClassWithSimpleProperties>();

        var autoFaker = new AutoFaker();

        // Act
        Parallel.For(0, numberOfTasks, _ =>
        {
            var fakeModel = autoFaker.Generate<TestClassWithSimpleProperties>();
            results.Add(fakeModel);
        });

        // Assert
        results.Count.Should().Be(numberOfTasks);

        List<int> ids = results.Select(x => x.Id).ToList();
        ids.Should().OnlyHaveUniqueItems();
    }

    [Fact]
    public async Task Generate_with_ParallelExecutionTasks()
    {
        // Arrange
        const int numberOfTasks = 1000;

        var autoFaker = new AutoFaker();

        // Act
        Task<TestClassWithSimpleProperties>[] tasks = Enumerable.Range(0, numberOfTasks)
            .Select(_ => Task.Run(() => autoFaker.Generate<TestClassWithSimpleProperties>()))
            .ToArray();

        TestClassWithSimpleProperties[] results = await Task.WhenAll(tasks);

        // Assert
        results.Length.Should().Be(numberOfTasks);

        List<int> ids = results.Select(x => x.Id).ToList();
        ids.Should().OnlyHaveUniqueItems();
    }
}