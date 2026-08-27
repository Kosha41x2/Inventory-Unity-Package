using UnityEngine;

namespace Kosha82.InventorySystem
{
    public interface IItemDisplayableImage
    {
        Sprite GetInInventoryDisplayImage();
        string GetInInventoryDisplayTag();
    }
}
