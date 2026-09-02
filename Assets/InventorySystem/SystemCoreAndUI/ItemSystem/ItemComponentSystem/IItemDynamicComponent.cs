using UnityEngine;

namespace Kosha82.InventorySystem
{
    /// <summary>
    /// Interface for item components that can provide dynamic behavior for inventory items.
    /// Dynamic components are those that can change their state or behavior at runtime, and may require special handling when stacking items in the inventory.
    /// An item that implements a dynamic component will be automatically treated as a unique instance, and will not stack with other items of the same type unless they have the same dynamic component state.
    /// </summary>
    public interface IItemDynamicComponent
    {
        IItemDynamicComponent CreateInstance(IItemDynamicComponent itemInstance);
        bool CanStackWith(IItemDynamicComponent otherComponent);
    }
}