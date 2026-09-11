using UnityEngine;

namespace Kosha82.InventorySystem.Crafting
{
    /// <summary>
    /// Represents a crafting recipe, which is a collection of ingredients that can be combined to create a new item. This class serves as a data container for the recipe and can be extended to include additional properties or methods as needed.
    /// </summary>
    [CreateAssetMenu(fileName = "Recipe", menuName = "Inventory System/Modules/Crafting/Recipe", order = 1)]
    public class Recipe : ScriptableObject
    {
        [SerializeField]private Vector2Int gridSize = new Vector2Int(3, 3);
        [SerializeField]private Ingredient[] ingredients;

        public Vector2Int GridSize => gridSize;
        public Ingredient[] Ingredients => ingredients;

        void OnValidate()
        {
            if (ingredients == null) ingredients = new Ingredient[gridSize.x * gridSize.y];
            if (gridSize.x < 1) gridSize.x = 1;
            if (gridSize.y < 1) gridSize.y = 1;

            else if(ingredients.Length != gridSize.x * gridSize.y)
            {
                System.Array.Resize(ref ingredients, gridSize.x * gridSize.y);
            }
        }

        void Reset()
        {
            ingredients = new Ingredient[gridSize.x * gridSize.y];
        }
    }
}
