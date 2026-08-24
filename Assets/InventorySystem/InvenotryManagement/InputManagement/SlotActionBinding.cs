using UnityEngine.UIElements;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]

public class InventorySlotDownEvent<T> : UnityEvent<Inventory, Vector2Int, VisualElement, PointerEventBase<T>> where T : PointerEventBase<T>, new() { }

[System.Serializable]
public class SlotActionBinding<T> where T : PointerEventBase<T>, new()
{
    [Tooltip("Check this if the action should trigger while dragging an item (you usually want this for drop actions in other slots).")]
    public bool whileDragging = false;

    [Tooltip("The needed mouse button to trigger the action (Left, Right, Middle). Ignored if it's a move event.")]
    public MouseButton requiredButton = MouseButton.LeftMouse;

    [Tooltip("The optional modifier key (Shift, Ctrl, Alt)")]
    public EventModifiers requiredModifier = EventModifiers.None;

    [Tooltip("The action to execute when the required button and modifier are pressed on a slot")]
    public InventorySlotDownEvent<T> actionToExecute;
}