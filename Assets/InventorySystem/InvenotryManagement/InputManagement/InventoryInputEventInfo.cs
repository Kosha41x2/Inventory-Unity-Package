using UnityEngine;
using UnityEngine.UIElements;
public class InventoryInputEventInfo
{
    public Inventory BackendInventory { get; private set; }
    public VisualElement SlotElement { get; private set; }
    public VisualElement Target { get; private set; }

    public string SlotClassName { get; private set; }

    public InventoryInputEventInfo(Inventory backendInventory, VisualElement slotElement, VisualElement target, string slotClassName = "itemSlot")
    {
        BackendInventory = backendInventory;
        SlotElement = slotElement;
        Target = target;
        SlotClassName = slotClassName;
    }

    public VisualElement GetVisualElementRoot()
    {
        return Target.panel.visualTree;
    }

    public bool HasSlotBeenClicked()
    {
        return SlotElement != null;
    }

    public Slot GetLogicalSlot()
    {
        if (SlotElement == null)
        {
            return null;
        }

        return BackendInventory.GetSlot((Vector2Int)SlotElement.dataSource);
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