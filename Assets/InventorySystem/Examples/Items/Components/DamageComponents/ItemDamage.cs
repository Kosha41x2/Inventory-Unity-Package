using UnityEngine;

namespace Kosha82.InventorySystem.Examples
{
    [CreateAssetMenu(fileName = "ItemDamage", menuName = "Inventory System/Item Components/Item Damage")]
    public class ItemDamage : ItemComponent, IItemDisplayableText, IItemDynamicComponent
    {
        [SerializeField]
        private string DisplayTag = "Damage";

        [SerializeField]
        private int damage = 1;

        public int Damage => damage;

        public string GetInInventoryDisplayText()
        {
            return $"Damage: {damage}";
        }

        public string GetInInventoryDisplayTag()
        {
            return DisplayTag;
        }

        public IItemDynamicComponent CreateInstance(IItemDynamicComponent itemDynamicComponent)
        {
            if(itemDynamicComponent is not ItemDamage itemDamage)
            {
                Debug.LogError("The provided itemDynamicComponent is not of type ItemDamage.");
                return null;
            }
            ItemDamage newItemDamage = ScriptableObject.Instantiate(itemDamage);
            return newItemDamage;
        }

        public bool CanStackWith(IItemDynamicComponent otherComponent)
        {
            if (otherComponent is ItemDamage otherDamage)
            {
                return this.damage == otherDamage.damage;
            }
            return false;
        }

        public void IncreaseDamage(int amount)
        {
            damage += amount;
        }
    }
}