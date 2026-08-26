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
            Vector2 slotPosition = slot.worldBound.center;
            float distance = Vector2.Distance(pointerPosition, slotPosition);

            if (distance < closestDistance && distance <= maxDistance)
            {
                closestDistance = distance;
                closestSlot = slot;
            }
        });

        return closestSlot;
    }
}
