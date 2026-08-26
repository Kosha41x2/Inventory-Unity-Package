using UnityEngine;
using UnityEngine.UIElements;
public class InventoryInputEventInfo
{
    public VisualElement ClickedSlot { get; private set; }
    public VisualElement Target { get; private set; }

    public string SlotClassName { get; private set; }

    public InventoryInputEventInfo(VisualElement slotElement, VisualElement target, string slotClassName = "itemSlot")
    {
        ClickedSlot = slotElement;
        Target = target;
        SlotClassName = slotClassName;
    }

    public VisualElement GetVisualElementRoot()
    {
        return Target.panel.visualTree;
    }

    public bool HasSlotBeenClicked()
    {
        return ClickedSlot != null;
    }

    public Slot GetLogicalSlot()
    {
        if (!HasSlotBeenClicked())
        {
            return null;
        }

        SlotDirection slotDirection = (SlotDirection)ClickedSlot.dataSource;

        return slotDirection.Inventory.GetSlot(slotDirection.Position);
    }
}

public class InventoryInputDownEventInfo
{
    public PointerDownEvent PointerEvent { get; private set; }

    public InventoryInputEventInfo ContextInfo { get; private set; }

    public InventoryInputDownEventInfo(PointerDownEvent pointerEvent, InventoryInputEventInfo contextInfo)
    {
        PointerEvent = pointerEvent;
        ContextInfo = contextInfo;
    }
}

public class InventoryInputMoveEventInfo
{
    public PointerMoveEvent PointerEvent { get; private set; }

    public InventoryInputEventInfo ContextInfo { get; private set; }

    public InventoryInputMoveEventInfo(PointerMoveEvent pointerEvent, InventoryInputEventInfo contextInfo)
    {
        PointerEvent = pointerEvent;
        ContextInfo = contextInfo;
    }
}

    public class InventoryInputUpEventInfo
    {
    public PointerUpEvent PointerEvent { get; private set; }

    public InventoryInputEventInfo ContextInfo { get; private set; }

    public InventoryInputUpEventInfo(PointerUpEvent pointerEvent, InventoryInputEventInfo contextInfo)
    {
        PointerEvent = pointerEvent;
        ContextInfo = contextInfo;
    }
}