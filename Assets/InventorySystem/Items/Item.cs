using UnityEngine;
using System.Collections.Generic;


namespace Kosha82.InventorySystem
{
    [CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
    public class Item : ScriptableObject
    {
        [SerializeField] private string itemName;
        [SerializeField] private Texture2D itemIcon;

        [SerializeField] private int itemID;

        [SerializeField] private int stackSize;

        [SerializeField] private List<ItemComponent> itemComponents = new List<ItemComponent>();

        public string ItemName => itemName;
        public Texture2D ItemIcon => itemIcon;
        public int ItemID => itemID;
        public int StackSize => stackSize;
        public IEnumerable<ItemComponent> ItemComponents => itemComponents;

        public T getComponent<T>() where T : ItemComponent
        {
            foreach (var component in itemComponents)
            {
                if (component is T typedComponent)
                {
                    return typedComponent;
                }
            }
            return null;
        }

        public List<T> getComponents<T>() where T : ItemComponent
        {
            List<T> components = new List<T>();
            foreach (var component in itemComponents)
            {
                if (component is T typedComponent)
                {
                    components.Add(typedComponent);
                }
            }
            return components;
        }

        public List<IItemDisplayableImage> GetDisplayableImageComponents()
        {
            List<IItemDisplayableImage> displayableImageComponents = new List<IItemDisplayableImage>();
            foreach (var component in itemComponents)
            {
                if (component is IItemDisplayableImage displayableImageComponent)
                {
                    displayableImageComponents.Add(displayableImageComponent);
                }
            }
            return displayableImageComponents;
        }

        public List<IItemDisplayableText> GetDisplayableTextComponents()
        {
            List<IItemDisplayableText> displayableTextComponents = new List<IItemDisplayableText>();
            foreach (var component in itemComponents)
            {
                if (component is IItemDisplayableText displayableTextComponent)
                {
                    displayableTextComponents.Add(displayableTextComponent);
                }
            }
            return displayableTextComponents;
        }
    }
}
