using System;
using System.Collections.Generic;
using Soenneker.Utils.AutoBogus.Tests.Dtos.Complex;

namespace Soenneker.Utils.AutoBogus.Tests.Dtos.Simple;

public class TestClassWithCollections
{
    // Collections of primitives - should be generated
    public List<string> StringList { get; set; }
    public ICollection<int> IntCollection { get; set; }
    public IEnumerable<Guid> GuidEnumerable { get; set; }
    
    // Collections of complex types - should be skipped when ShallowGenerate is enabled
    public List<OrderItem> OrderItemList { get; set; }
    public ICollection<Product> ProductCollection { get; set; }
    public IEnumerable<Order> OrderEnumerable { get; set; }
}

public class TestClassWithDictionaries
{
    // Dictionaries with primitive values - should be generated
    public Dictionary<string, int> StringIntDictionary { get; set; }
    public Dictionary<int, string> IntStringDictionary { get; set; }
    public IDictionary<string, Guid> StringGuidDictionary { get; set; }
    
    // Dictionaries with complex values - should be skipped when ShallowGenerate is enabled
    public Dictionary<string, OrderItem> StringOrderItemDictionary { get; set; }
    public Dictionary<int, Product> IntProductDictionary { get; set; }
    public IDictionary<string, Order> StringOrderDictionary { get; set; }
}

