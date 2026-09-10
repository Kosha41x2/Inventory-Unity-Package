using UnityEngine;

namespace Kosha82.InventorySystem.Crafting
{
    /// <summary>
    /// Represents a crafting recipe, which is a collection of ingredients that can be combined to create a new item. This class serves as a data container for the recipe and can be extended to include additional properties or methods as needed.
    /// </summary>
    [CreateAssetMenu(fileName = "Recipe", menuName = "Inventory System/Modules/Crafting/Recipe", order = 1)]
    public class Recipe : ScriptableObject
    {
    }
}
