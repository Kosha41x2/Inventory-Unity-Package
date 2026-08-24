using UnityEngine;
using UnityEngine.UIElements;
public class NewMonoBehaviourScript : MonoBehaviour
{
    public void SwapItems(Inventory inventory, VisualElement slotElement, PointerDownEvent evt)
    {
        Slot slot = inventory.GetSlot((Vector2Int)slotElement.dataSource);

        if (slot != null && !slot.IsEmpty())
        {
            InventoryUI.AdjustCursorFrame(slotElement);
            inventory.SwapDraggedSlot((Vector2Int)slotElement.dataSource);
            ItemManipulator.isDragging = true;
        }
    }

    public void DraggedFrameFollowCursor(Inventory inventory, VisualElement slotElement, PointerMoveEvent evt)
    {
        InventoryUI.UpdateCursorFramePosition(evt.position);
    }

    public void DropItem(Inventory inventory, VisualElement slotElement, PointerUpEvent evt)
    {
        if (slotElement == null) return;
        
        Slot slot = inventory.GetSlot((Vector2Int)slotElement.dataSource);

        if (slot != null)
        {
            inventory.SwapDraggedSlot((Vector2Int)slotElement.dataSource);
        }

        ItemManipulator.isDragging = false;
    }
}
