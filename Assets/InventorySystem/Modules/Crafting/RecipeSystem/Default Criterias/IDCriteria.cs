using UnityEngine;

namespace Kosha82.InventorySystem.Crafting
{

    /// <summary>
    /// Default criteria that indicates that certain ingredient requires a specific item to be present in the inventory slot. This criteria checks if the item in the slot matches the specified item ID.
    /// </summary>
    public class IDCriteria : Criteria
    {
        [SerializeField] private Item item;

        public override bool IsFulfilled(Inventory inventory, Vector2Int position)
        {
            Slot slot = inventory.GetSlot(position);
            return slot != null && !slot.IsEmpty() && slot.CurrentItem.ItemID == item.ItemID;
        }
    }
}