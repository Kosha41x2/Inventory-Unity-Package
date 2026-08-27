using UnityEngine;

namespace Kosha82.InventorySystem
{
    public struct SlotDirection
    {
        public Vector2Int Position;
        public Inventory Inventory;

        public SlotDirection(Vector2Int position, Inventory inventory)
        {
            Position = position;
            Inventory = inventory;
        }
    }
}
