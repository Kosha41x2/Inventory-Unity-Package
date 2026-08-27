using UnityEngine;

namespace Kosha82.InventorySystem
{
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
        public int AddItem(Item item, int amount)
        {
            if (item == null || amount <= 0) return amount;

            if (item == CurrentItem || CurrentItem == null)
            {
                CurrentItem = item;
                return AddAmount(amount);
            }
            else
            {
                return amount;
            }
        }

        /// <summary>
        /// Adds an amount to the current item in the slot. If the slot is empty or full, it will not add any amount.
        /// </summary>
        /// <param name="amount"></param>
        /// <returns>the remaining amount that could not be added</returns>
        public int AddAmount(int amount)
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
        /// <returns>the remaining amount that could not be removed</returns>
        public int RemoveItem(int amount)
        {
            CurrentAmount -= amount;
            int remainingAmount = Mathf.Max(0, -CurrentAmount);

            if (CurrentAmount <= 0)
            {
                ClearSlot();
            }

            return remainingAmount;
        }

        public void CopyFrom(Slot otherSlot)
        {
            CurrentItem = otherSlot.CurrentItem;
            CurrentAmount = otherSlot.CurrentAmount;
        }

        public void ClearSlot()
        {
            CurrentItem = null;
            CurrentAmount = 0;
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
        /// </summary>
        /// <param name="item"></param>
        /// <param name="amount"></param>
        public void ForceSet(Item item, int amount)
        {
            CurrentItem = item;
            CurrentAmount = amount;
        }

        public bool MergeWith(Slot otherSlot)
        {
            if (this.IsEmpty() || otherSlot.IsEmpty()) return false;

            if (this.CurrentItem != otherSlot.CurrentItem) return false;

            if (this.IsFull()) return false;

            int remainingAmount = this.AddAmount(otherSlot.CurrentAmount);

            otherSlot.ClearSlot();

            otherSlot.AddItem(this.CurrentItem, remainingAmount);

            return true;
        }
    }
}
