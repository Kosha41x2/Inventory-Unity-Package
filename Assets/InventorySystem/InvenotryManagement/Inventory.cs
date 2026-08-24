using NUnit.Framework;
using System;
using UnityEngine;
public class Inventory : MonoBehaviour
{
    [SerializeField] private int inventoryHorizontalSize = 10;
    [SerializeField] private int inventoryVerticalSize = 6;

    public int InventoryHorizontalSize => inventoryHorizontalSize;
    public int InventoryVerticalSize => inventoryVerticalSize;

    private Slot[,] slots;

    private Slot draggedSlot;

    public Slot DraggedSlot => draggedSlot;

    public event Action<Inventory> OnInventorySizeChanged;
    public event Action<Inventory> OnInventoryContentChanged;

    public event Action<Inventory, Vector2Int> OnSlotContentChanged;

    public event Action<Inventory> OnDraggedSlotContentChanged;
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

        draggedSlot = new Slot();
    }

    public void Awake()
    {
        InitializeInventory();
        OnInventorySizeChanged?.Invoke(this);
    }

    public void Start()
    {
        OnInventorySizeChanged?.Invoke(this);
        OnInventoryContentChanged?.Invoke(this);
    }

/// <summary>
/// Adds an item to the inventory. It first tries to add the item to existing slots that contain the same item. If there is still remaining amount, it will try to add the item to empty slots.
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
                    if(slot.IsFull()) continue;

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
/// <param name="posA"></param>
/// <param name="posB"></param>
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
        OnDraggedSlotContentChanged?.Invoke(this);
    }

    public Slot GetSlot(Vector2Int position)
    {
        return GetSlot(position.x, position.y);
    }
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
            OnDraggedSlotContentChanged?.Invoke(this);
        }

        return merged;
    }

    public int RemoveItemFromSlot(Vector2Int position, int amount)
    {
        Slot slot = GetSlot(position);
        if (slot != null)
        {
            int remaining = slot.RemoveItem(amount);
            OnSlotContentChanged?.Invoke(this, position); 
            return remaining;
        }

        return amount;
    }

    public int RemoveItemFromDraggedSlot(int amount)
    {
        if (draggedSlot != null)
        {
            int remaining = draggedSlot.RemoveItem(amount);
            OnDraggedSlotContentChanged?.Invoke(this);
            return remaining;
        }
        return amount;
    }

    public int AddItemToDraggedSlot(Item item, int amount)
    {
        if (draggedSlot != null)
        {
            int remaining = draggedSlot.AddItem(item, amount);
            OnDraggedSlotContentChanged?.Invoke(this);
            return remaining;
        }
        return amount;
    }

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

    public bool TransferFromSlotToDragged(Vector2Int position, int amount)
    {
        Slot sourceSlot = GetSlot(position);
        if (sourceSlot == null) return false;

        Item itemToTransfer = sourceSlot.CurrentItem;
        if (itemToTransfer == null) return false;

        if (sourceSlot.CurrentAmount < amount) return false;

        int remainingAmount = RemoveItemFromSlot(position, amount);
        
        AddItemToDraggedSlot(itemToTransfer, amount - remainingAmount);

        return remainingAmount == 0;
    }

    public bool TransferFromDraggedToSlot(Vector2Int position, int amount)
    {
        if (draggedSlot == null) return false;

        Item itemToTransfer = draggedSlot.CurrentItem;
        if (itemToTransfer == null) return false;
        if (draggedSlot.CurrentAmount < amount) return false;
        if(GetSlot(position).CurrentAmount + amount > itemToTransfer.StackSize) return false;

        int remainingAmount = RemoveItemFromDraggedSlot(amount);
        AddItemToSlot(position, itemToTransfer, amount - remainingAmount);
        
        return remainingAmount == 0;
    }
}
