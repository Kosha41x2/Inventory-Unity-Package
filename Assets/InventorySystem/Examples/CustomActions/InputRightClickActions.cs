using UnityEngine;
using UnityEngine.UIElements;

namespace Kosha82.InventorySystem.Examples
{
    public class InputRightClickActions : MonoBehaviour
    {
        [SerializeField] private float maxDistance = 100f;
        [SerializeField] private int amount = 1;

        public void GetSomeItems(InventoryInputDownEventInfo evt)
        {
            if (!evt.ContextInfo.HasSlotBeenClicked()) return;


            VisualElement slotElement = evt.ContextInfo.ClickedSlot;
            SlotDirection slotDirection = (SlotDirection)slotElement.dataSource;

            Slot slot = evt.ContextInfo.GetLogicalSlot();

            if (slot != null && !slot.IsEmpty())
            {
                CursorFrameUI.AdjustSizeToMatch(slotElement);
                CursorFrameUI.UpdatePosition(evt.PointerEvent.position);
                slotDirection.Inventory.TransferFromSlotToDragged(slotDirection.Position, amount);
                ItemManipulator.isDragging = true;
            }
        }

        public void DropSomeItems(InventoryInputDownEventInfo evt)
        {
            VisualElement slotElement = evt.ContextInfo.ClickedSlot;
            VisualElement root = evt.ContextInfo.GetVisualElementRoot();

            if (!evt.ContextInfo.HasSlotBeenClicked())
            {
                slotElement = AuxiliarInventoryInputFunc.FindClosestSlot(evt.PointerEvent.position, root, maxDistance, evt.ContextInfo.SlotClassName);
            }

            SlotDirection slotDirection = (SlotDirection)slotElement.dataSource;

            Slot slot = null;

            if (slotElement != null)
            {
                slot = slotDirection.Inventory.GetSlot(slotDirection.Position);
            }

            if (slot != null)
            {
                if (!slotDirection.Inventory.TransferFromDraggedToSlot(slotDirection.Position, amount))
                {
                    slotDirection.Inventory.SwapDraggedSlot(slotDirection.Position);
                }
            }

            ItemManipulator.isDragging = !Inventory.DraggedSlot.IsEmpty();
        }
    }
}