using UnityEngine.UIElements;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class SlotDownEvent : UnityEvent<Inventory, VisualElement, PointerDownEvent> { }

[System.Serializable]
public class SlotMoveEvent : UnityEvent<Inventory, VisualElement, PointerMoveEvent> { }

[System.Serializable]
public class SlotUpEvent : UnityEvent<Inventory, VisualElement, PointerUpEvent> { }

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