using UnityEngine;
using System;

namespace Kosha82.InventorySystem.Crafting
{

    /// <summary>
    /// Represents a criteria that must be fulfilled by an inventory slot for a crafting ingredient to be considered valid. This is an abstract base class that can be extended to define specific criteria, such as item type, quantity, or other conditions.
    /// </summary>
    [System.Serializable]
    public abstract class Criteria
    {
        /// <summary>
        /// Checks if the given inventory slot fulfills this criteria.
        /// </summary>
        /// <param name="inventory"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public abstract bool IsFulfilled(Inventory inventory, Vector2Int position);

        /// <summary>
        /// Consumes what's dictated by this criteria from the given inventory slot.
        /// </summary>
        /// <param name="inventory"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public virtual void Consume(Inventory inventory, Vector2Int position)
        {
            // Default implementation does nothing
        }
    }
}
