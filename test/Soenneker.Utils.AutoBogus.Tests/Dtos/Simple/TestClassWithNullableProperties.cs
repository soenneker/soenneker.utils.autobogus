using System;
using Soenneker.Utils.AutoBogus.Tests.Dtos.Complex;

namespace Soenneker.Utils.AutoBogus.Tests.Dtos.Simple;

public class TestClassWithNullableProperties
{
    // Nullable primitives - should be generated when ShallowGenerate is enabled
    public int? NullableInt { get; set; }
    public string? NullableString { get; set; }
    public Guid? NullableGuid { get; set; }
    public DateTime? NullableDateTime { get; set; }
    
    // Nullable complex types - should be skipped when ShallowGenerate is enabled
    public OrderItem? NullableOrderItem { get; set; }
    public Product? NullableProduct { get; set; }
    public Order? NullableOrder { get; set; }
}

