using NUnit.Framework;
using System;
using UnityEngine;

namespace Kosha82.InventorySystem
{

    /// <summary>
    /// Represents a matrix of slots that can hold items. It provides methods to control these slots,
    /// all these methods will automatically trigger events that will update the linked inventory UI.
    /// to link an inventory to an inventory UI, just drag this component to the inventory UI one or use the corresponding method in the inventory UI.
    /// </summary>
    [ExecuteAlways]
    [AddComponentMenu("Inventory System/Core/Inventory Backend")]
    public class Inventory : MonoBehaviour
    {
        [SerializeField] private int inventoryHorizontalSize = 10;
        [SerializeField] private int inventoryVerticalSize = 6;

        public int InventoryHorizontalSize => inventoryHorizontalSize;
        public int InventoryVerticalSize => inventoryVerticalSize;

        private Slot[,] slots;

        private static Slot draggedSlot;

        public static Slot DraggedSlot => draggedSlot;

        public event Action<Inventory> OnInventorySizeChanged;
        public event Action<Inventory> OnInventoryContentChanged;

        public event Action<Inventory, Vector2Int> OnSlotContentChanged;

        public event Action OnDraggedSlotContentChanged;

        /// <summary>
        /// Initializes the inventory according to inventory dimentions. Creating the 2D slot matrix.
        /// This method will also clear all the inventory slots and the dragged slot, so if no data is intended to be lost,
        /// make sure to save the inventory first somewhere else, this method is called automatically when the inventory is created
        /// or when the inventory dimentions are changed in the inspector or calling the corresponding methods.
        /// </summary>
        public void InitializeInventory()
        {
            slots = new Slot[inventoryHorizontalSize, inventoryVerticalSize];

            for (int x = 0; x < inventoryHorizontalSize; x++)
            {
                for (int y = 0; y < inventoryVerticalSize; y++)
                {
                    slots[x, y] = new Slot();
                }
            }

            if(draggedSlot == null)
            {
                draggedSlot = new Slot();
            }

            OnInventorySizeChanged?.Invoke(this);
        }
        private void OnValidate()
        {
            if (inventoryHorizontalSize < 1 || inventoryVerticalSize < 1) return;
            InitializeInventory();
        }

        private void Start()
        {
            OnInventorySizeChanged?.Invoke(this);
            OnInventoryContentChanged?.Invoke(this);
        }

        /// <summary>
        /// Changes the inventory size and reinitializes it. This will clear all the inventory slots and the dragged slot, so if no data is intended to be lost,
        /// make sure to save the inventory first somewhere else.
        /// </summary>
        /// <param name="newHorizontalSize"></param>
        /// <param name="newVerticalSize"></param>
        public void OnChangeSize(int newHorizontalSize, int newVerticalSize)
        {
            inventoryHorizontalSize = newHorizontalSize;
            inventoryVerticalSize = newVerticalSize;
            InitializeInventory();
        }

        /// <summary>
        /// Adds an item to the inventory. It first tries to add the item to existing slots that contain the same item. If there's no slot with the same item, it will try to add the item to empty slots.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="amount"></param>
        /// <returns>the remaining amount that could not be added</returns>
        public int AddItemToInventory(Item item, int amount)
        {
            for (int y = 0; y < inventoryVerticalSize && amount > 0; y++)
            {
                for (int x = 0; x < inventoryHorizontalSize && amount > 0; x++)
                {
                    Slot slot = GetSlot(x, y);
                    if (slot.CurrentItem == item)
                    {
                        if (slot.IsFull()) continue;

                        int remainingAmount = slot.AddItem(item, amount);
                        OnSlotContentChanged?.Invoke(this, new Vector2Int(x, y));

                        if (remainingAmount <= 0)
                        {
                            amount = 0;
                        }
                        else
                        {
                            amount = remainingAmount;
                        }
                    }
                }
            }

            for (int y = 0; y < inventoryVerticalSize && amount > 0; y++)
            {
                for (int x = 0; x < inventoryHorizontalSize && amount > 0; x++)
                {
                    Slot slot = GetSlot(x, y);
                    if (slot.IsEmpty())
                    {
                        int remainingAmount = slot.AddItem(item, amount);
                        OnSlotContentChanged?.Invoke(this, new Vector2Int(x, y));

                        if (remainingAmount <= 0)
                        {
                            amount = 0;
                        }
                        else
                        {
                            amount = remainingAmount;
                        }
                    }
                }
            }

            return amount;
        }

        /// <summary>
        /// Removes an item from the inventory. It will search for slots that contain the specified item and remove the specified amount from those slots.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="amount"></param>
        /// <returns>the remaining amount that could not be deleted</returns>
        public int DeleteItemFromInventory(Item item, int amount)
        {
            for (int y = 0; y < inventoryVerticalSize && amount > 0; y++)
            {
                for (int x = 0; x < inventoryHorizontalSize && amount > 0; x++)
                {
                    Slot slot = GetSlot(x, y);
                    if (slot.CurrentItem == item)
                    {
                        amount = slot.RemoveItem(amount);
                        OnSlotContentChanged?.Invoke(this, new Vector2Int(x, y));

                        if (amount <= 0)
                        {
                            amount = 0;
                        }
                    }
                }
            }

            return amount;
        }

        /// <summary>
        /// Swaps the contents of two slots in the inventory. If either slot is empty, it will simply copy the contents of the other slot to the empty one.
        /// </summary>
        /// <param name="posA">first slot position in the matrix</param>
        /// <param name="posB">second slot position in the matrix</param>
        public void SwapSlots(Vector2Int posA, Vector2Int posB)
        {
            Slot slotA = GetSlot(posA.x, posA.y);
            Slot slotB = GetSlot(posB.x, posB.y);

            if (slotA != null && slotB != null)
            {
                Slot tempSlot = new Slot();
                tempSlot.CopyFrom(slotA);
                slotA.CopyFrom(slotB);
                slotB.CopyFrom(tempSlot);
            }

            OnSlotContentChanged?.Invoke(this, posA);
            OnSlotContentChanged?.Invoke(this, posB);
        }

        /// <summary>
        /// Swaps the contents of the dragged slot with a specified slot in the inventory. If either slot is empty, it will simply copy the contents of the other slot to the empty one.
        /// </summary>
        /// <param name="posB">position of the slot to swap with</param>
        public void SwapDraggedSlot(Vector2Int posB)
        {
            Slot slotB = GetSlot(posB.x, posB.y);

            if (draggedSlot != null && slotB != null)
            {
                Slot tempSlot = new Slot();
                tempSlot.CopyFrom(draggedSlot);
                draggedSlot.CopyFrom(slotB);
                slotB.CopyFrom(tempSlot);
            }

            OnSlotContentChanged?.Invoke(this, posB);
            OnDraggedSlotContentChanged?.Invoke();
        }

        /// <summary>
        /// Retrieves a slot from the inventory based on its position. Returns null if the position is out of bounds.
        /// </summary>
        /// <param name="position">position of the slot to retrieve</param>
        /// <returns></returns>
        public Slot GetSlot(Vector2Int position)
        {
            return GetSlot(position.x, position.y);
        }

        /// <summary>
        /// Retrieves a slot from the inventory based on its x and y coordinates. Returns null if the coordinates are out of bounds.
        /// </summary>
        /// <param name="x">x-coordinate of the slot to retrieve</param>
        /// <param name="y">y-coordinate of the slot to retrieve</param>
        /// <returns></returns>
        public Slot GetSlot(int x, int y)
        {
            if (x >= 0 && x < inventoryHorizontalSize && y >= 0 && y < inventoryVerticalSize)
            {
                return slots[x, y];
            }
            else
            {
                Debug.LogWarning("Invalid slot coordinates.");
                return null;
            }
        }

        /// <summary>
        /// Merges the contents of two slots in the inventory. If the items in both slots are the same and the target slot has enough space, it will transfer as many items as possible from the source slot to the target slot.
        /// </summary>
        /// <param name="sourcePos">position of the source slot</param>
        /// <param name="targetPos">position of the target slot</param>
        /// <returns>success of the merge operation</returns>
        public bool MergeSlots(Vector2Int sourcePos, Vector2Int targetPos)
        {
            Slot sourceSlot = GetSlot(sourcePos);
            Slot targetSlot = GetSlot(targetPos);

            if (sourceSlot == null || targetSlot == null)
            {
                Debug.LogWarning("Invalid slot coordinates.");
                return false;
            }

            bool merged = targetSlot.MergeWith(sourceSlot);

            if (merged)
            {
                OnSlotContentChanged?.Invoke(this, sourcePos);
                OnSlotContentChanged?.Invoke(this, targetPos);
            }

            return merged;
        }

        /// <summary>
        /// Merges the contents of the dragged slot with a specified slot in the inventory. If the items in both slots are the same and the target slot has enough space, it will transfer as many items as possible from the dragged slot to the target slot.
        /// </summary>
        /// <param name="targetPos">position of the target slot</param>
        /// <returns></returns>
        public bool MergeSlotWithDragged(Vector2Int targetPos)
        {
            Slot targetSlot = GetSlot(targetPos);

            if (draggedSlot == null || targetSlot == null)
            {
                Debug.LogWarning("Invalid slot coordinates.");
                return false;
            }

            bool merged = targetSlot.MergeWith(draggedSlot);

            if (merged)
            {
                OnSlotContentChanged?.Invoke(this, targetPos);
                OnDraggedSlotContentChanged?.Invoke();
            }

            return merged;
        }

        /// <summary>
        /// Removes a specified amount of items from a slot in the inventory. If the slot has fewer items than the specified amount, it will remove all items from the slot and return the remaining amount that could not be removed.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="amount"></param>
        /// <param name="destroyItemIfEmptyAndDynamic">If the slot is empty and destroyItemIfEmptyAndDynamic is true, it will permanently destroy the item if it is a dynamic item.
        /// It is usually recomended to let the value of destroyItemIfEmptyAndDynamic to true, as it will prevent memory leaks when using dynamic items, as they are created at runtime.
        /// Only set it to false if you want to keep the item instance for some reason. (It does not affect non-dynamic items, as they are not created at runtime and are not destroyed when the slot is cleared)
        /// </param>
        /// <returns></returns>
        public int RemoveItemFromSlot(Vector2Int position, int amount, bool destroyItemIfEmptyAndDynamic = true)
        {
            Slot slot = GetSlot(position);
            if (slot != null)
            {
                int remaining = slot.RemoveItem(amount, destroyItemIfEmptyAndDynamic);
                OnSlotContentChanged?.Invoke(this, position);
                return remaining;
            }

            return amount;
        }

        /// <summary>
        /// Removes a specified amount of items from the dragged slot. If the dragged slot has fewer items than the specified amount, it will remove all items from the dragged slot and return the remaining amount that could not be removed.
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="destroyItemIfEmptyAndDynamic">If the dragged slot is empty and destroyItemIfEmptyAndDynamic is true, it will permanently destroy the item if it is a dynamic item.
        /// It is usually recomended to let the value of destroyItemIfEmptyAndDynamic to true, as it will prevent memory leaks when using dynamic items, as they are created at runtime.
        /// Only set it to false if you want to keep the item instance for some reason. (It does not affect non-dynamic items, as they are not created at runtime and are not destroyed when the slot is cleared)
        /// </param>
        /// <returns></returns>
        public int RemoveItemFromDraggedSlot(int amount, bool destroyItemIfEmptyAndDynamic = true)
        {
            if (draggedSlot != null)
            {
                int remaining = draggedSlot.RemoveItem(amount, destroyItemIfEmptyAndDynamic);
                OnDraggedSlotContentChanged?.Invoke();
                return remaining;
            }
            return amount;
        }


        /// <summary>
        /// Adds a specified amount of an item to the dragged slot. If the dragged slot already contains a different item, it will not add the new item and return the full amount. If the dragged slot is empty or contains the same item, it will add as many items as possible and return the remaining amount that could not be added.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public int AddItemToDraggedSlot(Item item, int amount)
        {
            if (draggedSlot != null)
            {
                int remaining = draggedSlot.AddItem(item, amount);
                OnDraggedSlotContentChanged?.Invoke();
                return remaining;
            }
            return amount;
        }

        /// <summary>
        /// Adds a specified amount of an item to a slot in the inventory. If the slot already contains a different item, it will not add the new item and return the full amount. If the slot is empty or contains the same item, it will add as many items as possible and return the remaining amount that could not be added.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="item"></param>
        /// <param name="amount"></param>
        /// <returns></returns>

        public int AddItemToSlot(Vector2Int position, Item item, int amount)
        {
            Slot slot = GetSlot(position);
            if (slot != null)
            {
                int remaining = slot.AddItem(item, amount);
                OnSlotContentChanged?.Invoke(this, position);
                return remaining;
            }
            return amount;
        }


        /// <summary>
        /// Transfers a specified amount of items from a slot in the inventory to the dragged slot. If the source slot has fewer items than the specified amount, it will transfer all items from the source slot and return true if at least an item was transferred.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="amount"></param>
        /// <returns></returns>

        public bool TransferFromSlotToDragged(Vector2Int position, int amount)
        {
            Slot sourceSlot = GetSlot(position);
            if (sourceSlot == null) return false;

            Item itemToTransfer = sourceSlot.CurrentItem;
            if (itemToTransfer == null) return false;
            if(!itemToTransfer.CanStackWith(draggedSlot.CurrentItem) && draggedSlot.CurrentItem != null) return false;

            amount = Math.Min(amount, sourceSlot.CurrentAmount);

            int remainingAmount = RemoveItemFromSlot(position, amount, false);

            AddItemToDraggedSlot(itemToTransfer, amount - remainingAmount);

            return remainingAmount == 0;
        }

        /// <summary>
        /// Transfers a specified amount of items from the dragged slot to a slot in the inventory. If the dragged slot has fewer items than the specified amount, it will transfer all items from the dragged slot and return if the transfer was successful.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="amount"></param>
        /// <returns>success on transfer</returns>
        public bool TransferFromDraggedToSlot(Vector2Int position, int amount)
        {
            if (draggedSlot == null) return false;

            Item itemToTransfer = draggedSlot.CurrentItem;
            if (itemToTransfer == null) return false;
            if(!itemToTransfer.CanStackWith(GetSlot(position).CurrentItem) && GetSlot(position).CurrentItem != null) return false;

            amount = Math.Min(amount, draggedSlot.CurrentAmount);

            if (GetSlot(position).CurrentAmount + amount > itemToTransfer.StackSize) return false;

            int remainingAmount = RemoveItemFromDraggedSlot(amount, false);
            AddItemToSlot(position, itemToTransfer, amount - remainingAmount);

            return remainingAmount == 0;
        }


        /// <summary>
        /// Forcefully sets the content of a slot in the inventory to a specified item and amount,
        /// it is not recommended to use, unless it's used for testing or if you know you're doing building
        /// a custom method yourself, but be aware of the fact that this method will not check the stack size or the current state of the slot,
        /// it will just set the item and amount and change the inventory UI accordingly.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="item"></param>
        /// <param name="amount"></param>
        public void ForceSetSlot(Vector2Int position, Item item, int amount)
        {
            Slot slot = GetSlot(position);
            if (slot != null)
            {
                slot.ForceSet(item, amount);
                OnSlotContentChanged?.Invoke(this, position);
            }
        }
    }
}