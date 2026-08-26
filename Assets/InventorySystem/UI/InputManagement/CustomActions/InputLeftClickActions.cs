using UnityEngine;
using UnityEngine.UIElements;
public class InputLeftClickActions : MonoBehaviour
{
    [SerializeField] private float maxDistance = 100f;
    public void SwapItems(InventoryInputDownEventInfo evt)
    {
        if (!evt.ContextInfo.HasSlotBeenClicked()) return;

        SlotDirection slotDirection = (SlotDirection)evt.ContextInfo.ClickedSlot.dataSource;
        VisualElement slotElement = evt.ContextInfo.ClickedSlot;

        Slot slot = slotDirection.Inventory.GetSlot(slotDirection.Position);

        if (slot != null && !slot.IsEmpty())
        {
            InventoryUI.AdjustCursorFrame(slotElement); // Needed just to adjust the cursor frame dimensions when starting to drag.
            slotDirection.Inventory.SwapDraggedSlot(slotDirection.Position);
            ItemManipulator.isDragging = true;
        }
    }

    public void DraggedFrameFollowCursor(InventoryInputMoveEventInfo evt)
    {
        InventoryUI.UpdateCursorFramePosition(evt.PointerEvent.position);
    }

    public void DropItems(InventoryInputDownEventInfo evt)
    {
        VisualElement slotElement = evt.ContextInfo.ClickedSlot;
        VisualElement root = evt.ContextInfo.GetVisualElementRoot();

        if (slotElement == null)
        {
            slotElement = AuxiliarInventoryInputFunc.FindClosestSlot(evt.PointerEvent.position, root, maxDistance, evt.ContextInfo.SlotClassName);
        }

        SlotDirection slotDirection = new SlotDirection(Vector2Int.zero, null);

        Slot slot = null;

        if(slotElement != null)
        {
            slotDirection = (SlotDirection)slotElement.dataSource;
            if(slotDirection.Inventory != null){
                slot = slotDirection.Inventory.GetSlot(slotDirection.Position);
            }
        }

        if (slot != null)
        {
            if(!slotDirection.Inventory.MergeSlotWithDragged(slotDirection.Position))
            {
                slotDirection.Inventory.SwapDraggedSlot(slotDirection.Position);
            }
        }

        ItemManipulator.isDragging = !Inventory.DraggedSlot.IsEmpty();
    }
}
