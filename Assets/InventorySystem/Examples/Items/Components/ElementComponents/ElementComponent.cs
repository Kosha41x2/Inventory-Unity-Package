using Kosha82.InventorySystem;
using UnityEngine;

namespace Kosha82.InventorySystem.Examples
{
    public enum ElementType
    {
        Fire,
        Water,
        Earth,
        Air,
        Lightning,
        Ice,
        Magic,
        Light,
        Dark
    }

    [CreateAssetMenu(fileName = "ElementComponent", menuName = "Inventory System/Item Components/Element Component")]
    public class ElementComponent : ItemComponent, IItemDisplayableImage
    {
        [SerializeField] Sprite inInventoryDisplayImage;
        [SerializeField] string inInventoryDisplayTag = "Element";
        [SerializeField] ElementType elementType;

        public ElementType ElementType => elementType;

        public Sprite GetInInventoryDisplayImage()
        {
            return inInventoryDisplayImage;
        }

        public string GetInInventoryDisplayTag()
        {
            return inInventoryDisplayTag;
        }
    }
}