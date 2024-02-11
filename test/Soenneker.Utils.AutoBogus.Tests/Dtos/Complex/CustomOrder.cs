using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Soenneker.Json.CollectionConverter;
using Soenneker.Utils.AutoBogus.Tests.Enums;

namespace Soenneker.Utils.AutoBogus.Tests.Dtos.Complex;

public sealed class CustomOrder : BaseCustomOrder
{
    public DateTime Timestamp;

    private int _privateFieldId;

    public const string Constant = "Order2x89ei";

    public string CustomId { get; set; }

    public int Id { get; }

    public ICalculator Calculator { get; }

    public Guid? Code { get; set; }

    public Status Status { get; set; }

    public DiscountBase[] Discounts { get; set; }

    public IEnumerable<OrderItem> Items { get; set; }

    public DateTimeOffset DateCreated { get; set; }

    public ICollection<string> Comments { get; set; }

    public List<DayOfWeekType> DaysOfWeek { get; set; }

    public Longitude Longitude { get; set; }

    [JsonPropertyName("daysOfWeek")]
    [System.Text.Json.Serialization.JsonConverter(typeof(CollectionConverter<SmartEnumNameConverter<DayOfWeekType, int>>))]
    [JsonProperty("daysOfWeek", ItemConverterType = typeof(Ardalis.SmartEnum.JsonNet.SmartEnumNameConverter<DayOfWeekType, int>))]
    public List<DayOfWeekType>? NullableDaysOfWeek { get; set; }

    public CustomOrder(int id, ICalculator calculator)
    {
        Id = id;
        Calculator = calculator;
    }
}