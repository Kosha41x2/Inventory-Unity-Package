using UnityEngine;

namespace Kosha82.InventorySystem
{
    /// <summary>
    /// Base class for all item components. Inherit from this class to create custom item components.
    /// </summary>
    public abstract class ItemComponent : ScriptableObject
    {
        private Item parentItem;

        public Item ParentItem => parentItem;

        /// <summary>
        /// Sets the parent item for this component. This is called automatically when the item is created or loaded, or a component is added to an item, and should not be called manually.
        /// </summary>
        /// <param name="parentItem"></param>
        public void SetParentItem(Item parentItem)
        {
            this.parentItem = parentItem;
        }
    }
}