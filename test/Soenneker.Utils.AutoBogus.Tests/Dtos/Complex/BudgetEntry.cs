using System;
using Bogus.DataSets;

namespace Soenneker.Utils.AutoBogus.Tests.Dtos.Complex;

public partial class BudgetEntry
{
    public int Id { get; set; }

    public int CurrencyId { get; set; }

    public decimal Amount { get; set; }

    public DateTime Date { get; set; }

    public int BudgetId { get; set; }

    public virtual Budget Budget { get; set; }

    public virtual Currency Currency { get; set; }
}