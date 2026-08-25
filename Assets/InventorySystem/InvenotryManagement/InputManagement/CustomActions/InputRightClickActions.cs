using UnityEngine;
using UnityEngine.UIElements;

public class InputRightClickActions : MonoBehaviour
{
    [SerializeField] private float maxDistance = 100f;
    [SerializeField] private int amount = 1;

    public void GetSomeItems(InventoryInputDownEventInfo evt)
    {
        if (!evt.ContextInfo.HasSlotBeenClicked()) return;

        Inventory inventory = evt.ContextInfo.BackendInventory;
        VisualElement slotElement = evt.ContextInfo.ClickedSlot;

        Slot slot = inventory.GetSlot((Vector2Int)slotElement.dataSource);

        if (slot != null && !slot.IsEmpty())
        {
            InventoryUI.AdjustCursorFrame(slotElement);
            inventory.TransferFromSlotToDragged((Vector2Int)slotElement.dataSource, amount);
            ItemManipulator.isDragging = true;
        }
    }

    public void DropSomeItems(InventoryInputDownEventInfo evt)
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
            if(!inventory.TransferFromDraggedToSlot((Vector2Int)slotElement.dataSource, amount))
            {
                inventory.SwapDraggedSlot((Vector2Int)slotElement.dataSource);
            }
        }

        ItemManipulator.isDragging = !Inventory.DraggedSlot.IsEmpty();
    }
}
