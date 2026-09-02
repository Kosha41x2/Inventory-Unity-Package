using UnityEngine;

namespace Kosha82.InventorySystem
{
    /// <summary>
    /// Interface for item components that can provide a displayable text and tag for inventory UI.
    /// Implement this interface in your custom item component scriptable objects to provide a specific text and
    /// tag.
    /// The tag will be used to map the style class name.
    /// </summary>
    public interface IItemDisplayableText
    {
        string GetInInventoryDisplayText();
        string GetInInventoryDisplayTag();
    }
}