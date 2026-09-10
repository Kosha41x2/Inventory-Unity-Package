using System;
using UnityEngine;
using System.Collections.Generic;



namespace Kosha82.InventorySystem.Crafting
{

    /// <summary>
    /// Represents an ingredient in a crafting recipe, which consists of a set of criteria that must be fulfilled by an inventory slot for the ingredient to be considered valid.
    /// </summary>
    [System.Serializable]
    public class Ingredient
    {
        [SerializeReference]
        List<Criteria> criterias = new List<Criteria>() {new IDCriteria(),
                                                        new QuantityCriteria()};

        /// <summary>
        /// Checks if the given inventory slot fulfills all the criteria defined in this ingredient.
        /// </summary>
        /// <param name="inventory"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool IsFulfilled(Inventory inventory, Vector2Int position)
        {
            foreach (var criteria in criterias)
            {
                if (!criteria.IsFulfilled(inventory, position))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Consumes what's dectated by the criteria in this ingredient from the given inventory slot.
        /// </summary>
        /// <param name="inventory"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public void Consume(Inventory inventory, Vector2Int position)
        {
            foreach (var criteria in criterias)
            {
                criteria.Consume(inventory, position);
            }
        }
    }
}