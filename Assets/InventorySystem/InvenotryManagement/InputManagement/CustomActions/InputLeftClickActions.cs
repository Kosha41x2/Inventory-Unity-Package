using UnityEngine;
using UnityEngine.UIElements;
public class InputLeftClickActions : MonoBehaviour
{
    [SerializeField] private float maxDistance = 100f;
    public void SwapItems(InventoryInputDownEventInfo evt)
    {
        if (!evt.ContextInfo.HasSlotBeenClicked()) return;

        Inventory inventory = evt.ContextInfo.BackendInventory;
        VisualElement slotElement = evt.ContextInfo.ClickedSlot;

        Slot slot = inventory.GetSlot((Vector2Int)slotElement.dataSource);

        if (slot != null && !slot.IsEmpty())
        {
            InventoryUI.AdjustCursorFrame(slotElement); // Needed just to adjust the cursor frame dimensions when starting to drag.
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
        VisualElement slotElement = evt.ContextInfo.ClickedSlot;
        VisualElement root = evt.ContextInfo.GetVisualElementRoot();

        if (slotElement == null)
        {
            slotElement = AuxiliarInventoryInputFunc.FindClosestSlot(evt.PointerEvent.position, root, maxDistance, evt.ContextInfo.SlotClassName);
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
}
