using UnityEngine;

namespace Kosha82.InventorySystem.Crafting
{

    /// <summary>
    /// Default criteria that indicates that certain ingredient requires a specific quantity of items to be present in the inventory slot. This criteria checks if the quantity of items in the slot meets or exceeds the specified required quantity.
    /// </summary>
    public class QuantityCriteria : Criteria
    {
        [Min(1)][SerializeField] private int requiredQuantity = 1;

        public override bool IsFulfilled(Inventory inventory, Vector2Int position)
        {
            Slot slot = inventory.GetSlot(position);
            return slot != null && !slot.IsEmpty() && slot.CurrentAmount >= requiredQuantity;
        }

        public override void Consume(Inventory inventory, Vector2Int position)
        {
            Slot slot = inventory.GetSlot(position);
            if (slot != null && !slot.IsEmpty())
            {
                inventory.RemoveItemFromSlot(position, requiredQuantity);
            }
        }
    }
}