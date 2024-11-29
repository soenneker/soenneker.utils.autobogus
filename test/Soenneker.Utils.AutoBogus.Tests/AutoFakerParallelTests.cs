using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        Assert.Equal(numberOfTasks, results.Count);

        List<int> ids = results.Select(x => x.Id).ToList();
        Assert.Equal(ids.Count, ids.Distinct().Count()); // Ensure IDs are unique
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
        Assert.Equal(numberOfTasks, results.Length);

        List<int> ids = results.Select(x => x.Id).ToList();
        Assert.Equal(ids.Count, ids.Distinct().Count()); // Ensure IDs are unique
    }
}