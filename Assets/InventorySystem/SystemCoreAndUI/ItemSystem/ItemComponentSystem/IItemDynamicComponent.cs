using UnityEngine;

namespace Kosha82.InventorySystem
{
    public interface IItemDynamicComponent
    {
        IItemDynamicComponent CreateInstance(IItemDynamicComponent itemInstance);
        bool CanStackWith(IItemDynamicComponent otherComponent);
    }
}