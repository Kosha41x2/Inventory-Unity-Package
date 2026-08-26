using UnityEngine.UIElements;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
[Tooltip("A binding that associates a specific pointer action (down) with a UnityEvent.")]
public class SlotDownEvent : UnityEvent<InventoryInputDownEventInfo> { }

[System.Serializable]
[Tooltip("A binding that associates a specific pointer action (move) with a UnityEvent.")]
public class SlotMoveEvent : UnityEvent<InventoryInputMoveEventInfo> { }

[System.Serializable]
[Tooltip("A binding that associates a specific pointer action (up) with a UnityEvent.")]
public class SlotUpEvent : UnityEvent<InventoryInputUpEventInfo> { }

[System.Serializable]
public class SlotActionBindingBase
{
    [Tooltip("Check this if the action should trigger while dragging an item (usually used to move or drop items in the inventory).")]
    public bool whileDragging = false;

    [Tooltip("The optional modifier key (Shift, Ctrl, Alt)")]
    public EventModifiers requiredModifier = EventModifiers.None;
}

[System.Serializable]
public class SlotDownBinding : SlotActionBindingBase
{
    [Tooltip("The needed mouse button to trigger the action.")]
    public MouseButton requiredButton = MouseButton.LeftMouse;
    public SlotDownEvent action;
}

[System.Serializable]
public class SlotMoveBinding : SlotActionBindingBase
{
    public SlotMoveEvent action;
}

[System.Serializable]
public class SlotUpBinding : SlotActionBindingBase
{
    public MouseButton requiredButton = MouseButton.LeftMouse;
    public SlotUpEvent action;
}