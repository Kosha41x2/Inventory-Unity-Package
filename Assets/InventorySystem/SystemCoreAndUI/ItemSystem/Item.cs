using UnityEngine;
using System.Collections.Generic;


namespace Kosha82.InventorySystem
{
    [CreateAssetMenu(fileName = "Item", menuName = "Inventory System/Item")]
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

        /// <summary>
        /// Indicates whether this item has any dynamic components. Dynamic components are those that implement the IItemDynamicComponent interface.
        /// </summary>
        /// <value></value>
        public bool IsDynamic {get; private set;} = false;
        public bool IsACopy {get; private set;} = false;

        void OnEnable()
        {
            IsDynamic = HasDynamicComponents();
        }

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

        public List<IItemDynamicComponent> GetDynamicComponents()
        {
            List<IItemDynamicComponent> dynamicComponents = new List<IItemDynamicComponent>();
            foreach (var component in itemComponents)
            {
                if (component is IItemDynamicComponent dynamicComponent)
                {
                    dynamicComponents.Add(dynamicComponent);
                }
            }
            return dynamicComponents;
        }

        /// <summary>
        /// Checks if this item can stack with another item. Two items can stack if they have the same ItemID and all their dynamic components can stack with each other.
        /// If no dynamic components are present, the items can stack if they have the same ItemID.
        /// </summary>
        /// <param name="otherItem"></param>
        /// <returns></returns>
        public bool CanStackWith(Item otherItem)
        {
            if (otherItem == null) return false;
            if (this.ItemID != otherItem.ItemID) return false;
            if (!this.IsDynamic && !otherItem.IsDynamic) return true; // If neither item has dynamic components, they can stack

            List<IItemDynamicComponent> thisDynamicComponents = this.GetDynamicComponents();
            List<IItemDynamicComponent> otherDynamicComponents = otherItem.GetDynamicComponents();

            if (thisDynamicComponents.Count != otherDynamicComponents.Count) return false;

            for (int i = 0; i < thisDynamicComponents.Count; i++)
            {
                if (!thisDynamicComponents[i].CanStackWith(otherDynamicComponents[i]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Checks if this item has any dynamic components. Dynamic components are those that implement the IItemDynamicComponent interface.
        /// </summary>
        /// <returns></returns>
        private bool HasDynamicComponents()
        {
            foreach (var component in itemComponents)
            {
                if (component is IItemDynamicComponent)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Creates a new instance of this item. If the item has dynamic components, it will create new instances of those components as well.
        /// This is useful for creating unique instances of items that can have different states or properties at runtime, such as weapons with different durability or potions with different effects.
        /// </summary>
        /// <returns></returns>
        public Item CreateInstance()
        {
            Item newItem = ScriptableObject.Instantiate(this);

            newItem.itemComponents = new List<ItemComponent>();

            foreach (var component in this.itemComponents)
            {
                if(component is IItemDynamicComponent dynamicComponent)
                {
                    var newComponent = dynamicComponent.CreateInstance(dynamicComponent);
                    newItem.itemComponents.Add(newComponent as ItemComponent);
                }
                else
                {
                    var newComponent = component;
                    newItem.itemComponents.Add(newComponent);
                }
            }
            newItem.IsACopy = true;
            return newItem;
        }

        void OnDestroy()
        {
            foreach (var component in itemComponents)
            {
                if (component is IItemDynamicComponent dynamicComponent)
                {
                    Object.Destroy(dynamicComponent as Object);
                }
            }
        }
    }
}
