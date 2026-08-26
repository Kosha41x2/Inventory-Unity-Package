using UnityEngine;
using UnityEngine.UIElements;

public static class AuxiliarInventoryInputFunc
{
    /// <summary>
    /// Finds the closest slot to the pointer position within a specified maximum distance.
    /// </summary>
    /// <param name="pointerPosition"></param>
    /// <param name="root"> The root visual element to search within </param>
    /// <param name="maxDistance"> Maximum distance to consider slots within</param>
    /// <param name="slotClassName"></param>
    /// <returns></returns>
    public static VisualElement FindClosestSlot(Vector2 pointerPosition, VisualElement root, float maxDistance, string slotClassName = "itemSlot")
    {
        VisualElement closestSlot = null;
        float closestDistance = float.MaxValue;

        root.Query<VisualElement>(className: slotClassName).ForEach(slot =>
        {
            if(IsElementVisibleAndUnclipped(slot))
            {
                Vector2 slotPosition = root.WorldToLocal(slot.worldBound.center);
                float distance = Vector2.Distance(pointerPosition, slotPosition);

                if (distance < closestDistance && distance <= maxDistance)
                {
                    closestDistance = distance;
                    closestSlot = slot;
                }
            }
        });

        return closestSlot;
    }

    /// <summary>
    /// Checks if a VisualElement is visible and not clipped by any of its parent elements, including ScrollViews.
    /// </summary>
    private static bool IsElementVisibleAndUnclipped(VisualElement element)
    {
        if (element.resolvedStyle.display == DisplayStyle.None || 
            element.resolvedStyle.visibility == Visibility.Hidden)
        {
            return false;
        }

        Vector2 elementCenter = element.worldBound.center;
        VisualElement currentParent = element.parent;

        while (currentParent != null)
        {
            // If the parent is a ScrollView or has a class indicating it's a scroll view content viewport, check if the element's center is within its bounds.
            if (currentParent is ScrollView || currentParent.ClassListContains("unity-scroll-view__content-viewport"))
            {
                // If the center of the slot is outside the bounds of the ScrollView, it's hidden
                if (!currentParent.worldBound.Contains(elementCenter))
                {
                    return false;
                }
            }
            currentParent = currentParent.parent;
        }

        return true;
    }
}
