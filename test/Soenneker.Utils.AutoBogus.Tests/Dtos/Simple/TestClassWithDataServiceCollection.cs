using Microsoft.OData.Client;

namespace Soenneker.Utils.AutoBogus.Tests.Dtos.Simple;

public class TestClassWithDataServiceCollection
{
    public DataServiceCollection<string> Collection { get; set; }
}
