using System;
using Soenneker.Utils.AutoBogus.Tests.Playground.Dtos;

namespace Soenneker.Utils.AutoBogus.Tests.Playground;

public sealed class ItemFaker
    : AutoFaker<Item>
{
    public ItemFaker(Guid id)
    {
        RuleFor(item => item.Id, () => id);
        RuleFor(item => item.Status, () => ItemStatus.Pending);
    }
}