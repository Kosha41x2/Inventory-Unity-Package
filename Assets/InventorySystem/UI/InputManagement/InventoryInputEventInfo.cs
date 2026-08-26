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

    /// <summary>
    /// Gets the root visual element of the target.
    /// </summary>
    /// <returns>The root visual element of the target.</returns>
    public VisualElement GetVisualElementRoot()
    {
        return Target.panel.visualTree;
    }

    /// <summary>
    /// Checks if a slot has been clicked.
    /// </summary>
    /// <returns></returns>
    public bool HasSlotBeenClicked()
    {
        return ClickedSlot != null;
    }

    /// <summary>
    /// Gets the logical slot associated with the clicked slot.
    /// </summary>
    /// <returns></returns>
    public Slot GetLogicalSlot()
    {
        if (!HasSlotBeenClicked()) return null;
        SlotDirection slotDirection = (SlotDirection)ClickedSlot.dataSource;
        return slotDirection.Inventory.GetSlot(slotDirection.Position);
    }

    /// <summary>
    /// Gets the backend inventory associated with the clicked slot.
    /// if no slot has been clicked, returns null.
    /// So it's recommended to check if a slot has been clicked before or
    /// to get first the SlotDirection and then get the inventory from it.
    /// </summary>
    /// <returns></returns>
    public Inventory GetBackendInventory()
    {
        if (!HasSlotBeenClicked()) return null;
        SlotDirection slotDirection = (SlotDirection)ClickedSlot.dataSource;
        return slotDirection.Inventory;
    }

    /// <summary>
    /// Gets the position of the clicked slot.
    /// If no slot has been clicked, returns Vector2Int.zero.
    /// So it's recommended to check if a slot has been clicked before or
    /// to get first the SlotDirection and then get the position from it.
    /// </summary>
    /// <returns></returns>
    public Vector2Int GetSlotPosition()
    {
        if (!HasSlotBeenClicked()) return Vector2Int.zero;
        SlotDirection slotDirection = (SlotDirection)ClickedSlot.dataSource;
        return slotDirection.Position;
    }

    /// <summary>
    /// Gets the SlotDirection associated with the clicked slot.
    /// If no slot has been clicked, returns a default SlotDirection with Vector2Int.zero and null inventory.
    /// </summary>
    /// <returns></returns>
    public SlotDirection GetSlotDirection()
    {
        if (!HasSlotBeenClicked()) return new SlotDirection(Vector2Int.zero, null);
        return (SlotDirection)ClickedSlot.dataSource;
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