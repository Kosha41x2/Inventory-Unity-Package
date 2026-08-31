using UnityEngine;

namespace Kosha82.InventorySystem
{

    /// <summary>
    /// Represents a single slot with an item and its amount. It is read only outside of this assembly (asmdef),
    /// and all modifications must be done through the Inventory class, to make sure that
    /// in case a UI is used, all the events are triggered and the inventory UI is updated accordingly.
    /// </summary>
    public class Slot
    {
        public Item CurrentItem { get; private set; }
        public int CurrentAmount { get; private set; }

        public Slot()
        {
            CurrentItem = null;
            CurrentAmount = 0;
        }

        /// <summary>
        /// Adds an item to the slot. If the slot is empty or contains the same item, it will add the amount to the slot.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="amount"></param>
        /// <returns>the remaining amount that could not be added</returns>
        internal int AddItem(Item item, int amount)
        {
            if (item == null || amount <= 0) return amount;

            if (!(item.CanStackWith(CurrentItem) || CurrentItem == null)) return amount;

            if(CurrentItem == null)
            {
                if(item.IsDynamic)
                {
                    item = item.CreateInstance();
                }
                CurrentItem = item;
            }
            
            return AddAmount(amount);
        }

        /// <summary>
        /// Adds an amount to the current item in the slot. If the slot is empty or full, it will not add any amount.
        /// </summary>
        /// <param name="amount"></param>
        /// <returns>the remaining amount that could not be added</returns>
        internal int AddAmount(int amount)
        {
            if (CurrentItem != null && !IsFull())
            {
                int spaceLeft = CurrentItem.StackSize - CurrentAmount;
                int amountToAdd = Mathf.Min(spaceLeft, amount);
                CurrentAmount += amountToAdd;
                return amount - amountToAdd;
            }
            else
            {
                return amount;
            }
        }

        /// <summary>
        /// Removes an amount from the current item in the slot. If the slot is empty, it will not remove any amount.
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="destroyItemIfEmptyAndDynamic">
        /// If the slot is empty and destroyItemIfEmptyAndDynamic is true, it will permanently destroy the item if it is a dynamic item.
        /// It is usually recomended to let the value of destroyItemIfEmptyAndDynamic to true, as it will prevent memory leaks when using dynamic items, as they are created at runtime.
        /// Only set it to false if you want to keep the item instance for some reason. (It does not affect non-dynamic items, as they are not created at runtime and are not destroyed when the slot is cleared)
        /// </param>
        /// <returns>the remaining amount that could not be removed</returns>
        internal int RemoveItem(int amount, bool destroyItemIfEmptyAndDynamic = true)
        {
            CurrentAmount -= amount;
            int remainingAmount = Mathf.Max(0, -CurrentAmount);

            if (CurrentAmount <= 0)
            {
                SmartClearSlot(destroyItemIfEmptyAndDynamic);
            }

            return remainingAmount;
        }

        /// <summary>
        /// Copies the item and amount from another slot to this slot.
        /// It copies the reference of the item, never use this method to copy the item itself, use the CreateInstance method of the item instead.
        /// </summary>
        /// <param name="otherSlot"></param>
        internal void CopyFrom(Slot otherSlot)
        {
            CurrentItem = otherSlot.CurrentItem;
            CurrentAmount = otherSlot.CurrentAmount;
        }

        /// <summary>
        /// Copies the item and amount from another slot to this slot, and creates a new instance
        /// of the item using the CreateInstance method of the item. This is useful when you want to copy a slot but not share the same item reference.
        /// </summary>
        /// <param name="otherSlot"></param>
        /// <returns></returns>
        internal void CopyFromAndInstantiate(Slot otherSlot)
        {
            CurrentItem = otherSlot.CurrentItem?.CreateInstance();
            CurrentAmount = otherSlot.CurrentAmount;
        }



        internal void ClearAndDestroySlot()
        {
            if(CurrentItem == null || !CurrentItem.IsACopy) return;
            Object.Destroy(CurrentItem);
            CurrentItem = null;
            CurrentAmount = 0;
        }

        internal void ClearItemReference()
        {
            CurrentItem = null;
            CurrentAmount = 0;
        }

        /// <summary>
        /// Clears the slot. If the item is dynamic, it will destroy the item instance. If the item is not dynamic, it will only clear the reference to the item.
        /// This is to prevent memory leaks when using dynamic items, as they are created at runtime
        /// </summary>
        internal void SmartClearSlot(bool destryItemIfDynamic = true)
        {
            if(CurrentItem != null && CurrentItem.IsDynamic && destryItemIfDynamic)
            {
                ClearAndDestroySlot();
            }
            else
            {
                ClearItemReference();
            }
        }

        public bool IsEmpty()
        {
            return CurrentItem == null;
        }

        public bool IsFull()
        {
            return CurrentItem != null && CurrentAmount >= CurrentItem.StackSize;
        }


        /// <summary>
        /// Forcefully sets the item and amount in the slot, regardless of the current state.
        /// This method is only for developer testing and should not be used in production code.
        /// It doesn't instanciate a new item, so it can cause reference issues.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="amount"></param>
        internal void ForceSet(Item item, int amount)
        {
            CurrentItem = item;
            CurrentAmount = amount;
        }

        /// <summary>
        /// Merges the current slot with another slot. If both slots contain the same item and the current slot is not full, it will transfer as many items as possible from the other slot to the current slot.
        /// If the current slot is full or the items are different, it will not transfer any items and return false. If the transfer is successful, it will return true.
        /// </summary>
        /// <param name="otherSlot"></param>
        /// <param name="destroyDynamicItemIfEmpty">If the slot is empty and destroyDynamicItemIfEmpty is true, it will permanently destroy the item if it is a dynamic item.
        /// It is usually recomended to let the value of destroyDynamicItemIfEmpty to true, as it will prevent memory leaks when using dynamic items, as they are created at runtime.
        /// Only set it to false if you want to keep the item instance for some reason. (It does not affect non-dynamic items, as they are not created at runtime and are not destroyed when the slot is cleared)
        /// </param>
        /// <returns></returns>
        internal bool MergeWith(Slot otherSlot, bool destroyDynamicItemIfEmpty = true)
        {
            if (this.IsEmpty() || otherSlot.IsEmpty()) return false;

            if (!this.CurrentItem.CanStackWith(otherSlot.CurrentItem)) return false;

            if (this.IsFull()) return false;

            int remainingAmount = this.AddAmount(otherSlot.CurrentAmount);

            if (remainingAmount <= 0) 
            {
                otherSlot.SmartClearSlot(destroyDynamicItemIfEmpty);
            } 
            else
            {
                otherSlot.ForceSet(otherSlot.CurrentItem, remainingAmount); 
            }

            return true;
        }
    }
}
