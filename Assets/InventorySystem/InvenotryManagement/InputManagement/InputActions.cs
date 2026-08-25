using UnityEngine;
using UnityEngine.UIElements;
public class InputActions : MonoBehaviour
{
    [SerializeField] private float maxDistance = 100f;
    public void SwapItems(InventoryInputDownEventInfo evt)
    {
        if (!evt.ContextInfo.HasSlotBeenClicked()) return;

        Inventory inventory = evt.ContextInfo.BackendInventory;
        VisualElement slotElement = evt.ContextInfo.SlotElement;

        Slot slot = inventory.GetSlot((Vector2Int)slotElement.dataSource);

        if (slot != null && !slot.IsEmpty())
        {
            InventoryUI.AdjustCursorFrame(slotElement);
            inventory.SwapDraggedSlot((Vector2Int)slotElement.dataSource);
            ItemManipulator.isDragging = true;
        }
    }

    public void DraggedFrameFollowCursor(InventoryInputMoveEventInfo evt)
    {
        InventoryUI.UpdateCursorFramePosition(evt.PointerEvent.position);
    }

    public void DropItems(InventoryInputDownEventInfo evt)
    {
        Inventory inventory = evt.ContextInfo.BackendInventory;
        VisualElement slotElement = evt.ContextInfo.SlotElement;
        VisualElement root = evt.ContextInfo.GetVisualElementRoot();

        if (slotElement == null)
        {
            slotElement = FindClosestSlot(evt.PointerEvent.position, root, evt.ContextInfo.SlotClassName);
        }

        Slot slot = null;

        if(slotElement != null)
        {
            slot = inventory.GetSlot((Vector2Int)slotElement.dataSource);
        }

        if (slot != null)
        {
            if(!inventory.MergeSlotWithDragged((Vector2Int)slotElement.dataSource))
            {
                inventory.SwapDraggedSlot((Vector2Int)slotElement.dataSource);
            }
        }

        ItemManipulator.isDragging = !inventory.DraggedSlot.IsEmpty();
    }

    private VisualElement FindClosestSlot(Vector2 pointerPosition, VisualElement root, string slotClassName = "itemSlot")
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
