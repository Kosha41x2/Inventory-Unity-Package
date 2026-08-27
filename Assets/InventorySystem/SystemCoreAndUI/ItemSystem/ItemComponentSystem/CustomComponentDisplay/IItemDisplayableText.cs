using UnityEngine;

namespace Kosha82.InventorySystem
{
    public interface IItemDisplayableText
    {
        string GetInInventoryDisplayText();
        string GetInInventoryDisplayTag();
    }
}