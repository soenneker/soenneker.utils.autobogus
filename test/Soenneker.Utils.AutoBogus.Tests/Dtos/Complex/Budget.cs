using System.Collections.Generic;
using Soenneker.Utils.AutoBogus.Tests.Dtos.Complex.Abstract;

namespace Soenneker.Utils.AutoBogus.Tests.Dtos.Complex;

public partial class Budget : ISoftDeleted
{
    public int Id { get; set; }

    public string Name { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public ICollection<BudgetEntry> BudgetEntries { get; set; }
    public Budget()
    {
        BudgetEntries = new HashSet<BudgetEntry>();
    }
}