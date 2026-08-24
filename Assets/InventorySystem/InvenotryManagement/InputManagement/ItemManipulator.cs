using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

/// <summary>
/// Handles pointer interactions for dragging and dropping items within the inventory UI without capturing the pointer.
/// </summary>
public class ItemManipulator : PointerManipulator
{
    private double maxInteractionDistance = 100.0;
    private VisualElement originalSlot;
    
    private Inventory backendInventory;

    private List<SlotActionBinding> slotActionBindings = new List<SlotActionBinding>();
    private bool isDragging = false;

    /// <summary>
    /// Initializes a new instance of the ItemManipulator class.
    /// </summary>
    /// <param name="inventory">The backend inventory reference.</param>
    /// <param name="slotActionBindings">The list of slot action bindings.</param>
    public ItemManipulator(Inventory inventory, List<SlotActionBinding> slotActionBindings)
    {
        this.backendInventory = inventory;
        this.slotActionBindings = slotActionBindings;
    }
    
    /// <summary>
    /// Registers the local pointer down event to start dragging.
    /// </summary>
    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<PointerDownEvent>(OnPointerDown);
    }

    /// <summary>
    /// Unregisters local callbacks and ensures global callbacks are cleaned up to prevent memory leaks.
    /// </summary>
    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
        
        if (isDragging && target?.panel != null)
        {
            target.panel.visualTree.UnregisterCallback<PointerMoveEvent>(OnGlobalPointerMove);
            target.panel.visualTree.UnregisterCallback<PointerDownEvent>(OnGlobalPointerDown, TrickleDown.TrickleDown);
        }
    }

    /// <summary>
    /// Handles the local pointer down event to determine whether to pick up an item.
    /// </summary>
    /// <param name="evt">The pointer down event data.</param>
    private void OnPointerDown(PointerDownEvent evt)
    {
        if (isDragging || !backendInventory.DraggedSlot.IsEmpty()) return;

        if (target == null) return;
        originalSlot = target.parent;
        
        if (originalSlot?.dataSource is not Vector2Int) return;

        if (backendInventory.GetSlot((Vector2Int)originalSlot.dataSource).IsEmpty()) return;

        switch(evt.button)
        {
            case (int)MouseButton.LeftMouse:
                PickUpStack();
                break;
            case (int)MouseButton.RightMouse:
                PickUpItems(1);
                break;
            case (int)MouseButton.MiddleMouse:
                PickUpPartialStack(0.5f);
                break;
            default:
                return;
        }

        
        if (InventoryUI.CursorFrame != null)
        {
            InventoryUI.CursorFrame.style.width = target.layout.width;
            InventoryUI.CursorFrame.style.height = target.layout.height;
            UpdateCursorPosition(evt.position);
        }

        isDragging = true;

        target.panel.visualTree.RegisterCallback<PointerMoveEvent>(OnGlobalPointerMove);
        target.panel.visualTree.RegisterCallback<PointerDownEvent>(OnGlobalPointerDown, TrickleDown.TrickleDown);
        
        evt.StopPropagation(); 
    }

    /// <summary>
    /// Picks up the entire stack, updates the backend, and subscribes to global events for tracking.
    /// </summary>
    /// <param name="evt">The pointer down event data.</param>
    private void PickUpStack()
    {
        backendInventory.SwapDraggedSlot((Vector2Int)originalSlot.dataSource);
    }

    /// <summary>
    /// Picks up a specified amount of items from the original slot and updates the backend inventory.
    /// </summary>
    /// <param name="amount">The number of items to pick up.</param>
    private void PickUpItems(int amount)
    {
        backendInventory.TransferFromSlotToDragged((Vector2Int)originalSlot.dataSource, amount);
    }


    /// <summary>
    /// Picks up half of the stack from the original slot and updates the backend inventory.
    /// </summary>
    /// <param name="factor">The fraction of the stack to pick up (default is 0.5 for half).</param>
    private void PickUpPartialStack(float factor = 0.5f)
    {
        Slot slot = backendInventory.GetSlot((Vector2Int)originalSlot.dataSource);
        if (slot == null || slot.IsEmpty()) return;

        int halfAmount = Mathf.CeilToInt(slot.CurrentAmount * factor);
        backendInventory.TransferFromSlotToDragged((Vector2Int)originalSlot.dataSource, halfAmount);
    }

    /// <summary>
    /// Global listener that updates the cursor frame's position every frame.
    /// </summary>
    /// <param name="evt">The pointer move event data.</param>
    private void OnGlobalPointerMove(PointerMoveEvent evt)
    {
        UpdateCursorPosition(evt.position);
    }

    /// <summary>
    /// Global listener that intercepts the next click to determine the drop target and action.
    /// </summary>
    /// <param name="evt">The pointer down event data.</param>
    private void OnGlobalPointerDown(PointerDownEvent evt)
    {
        if (!isDragging) return;

        VisualElement elementUnderPointer = target.panel.Pick(evt.position);
        VisualElement slotToChange = FindParentSlot(elementUnderPointer);

        if (slotToChange == null)
        {
            slotToChange = FindClosestSlot(evt.position);
        }

        if (slotToChange == null) slotToChange = originalSlot;

        Vector2Int targetPos = (Vector2Int)slotToChange.dataSource;

        switch(evt.button)
        {
            case (int)MouseButton.LeftMouse:
                DropAllItems(targetPos);
                break;
            case (int)MouseButton.RightMouse:
                DropSomeItems(targetPos, 1);
                break;
            case (int)MouseButton.MiddleMouse:
                DropPartialStack(targetPos, 0.5f);
                break;
            default:
                return;
        }

        evt.StopPropagation(); 

        if (backendInventory.DraggedSlot.IsEmpty())
        {
            StopDragging();
        }
    }

    /// <summary>
    /// Draps items into the target slot, if only some of them can be dropped, it will drop as many as possible and leave the rest in the dragged slot.
    /// If the target slot is empty, full or items are different, it will simply swap the dragged slot with the target
    /// </summary>
    /// <param name="targetPos">The calculated drop target position.</param>
    private void DropAllItems(Vector2Int targetPos)
    {
        if (!backendInventory.MergeSlotWithDragged(targetPos))
        {
            backendInventory.SwapDraggedSlot(targetPos);
        }
    }

    /// <summary>
    /// Drops a specified amount of items into the target slot. If the target slot is empty
    /// or has the same item type and enough space, it will transfer the specified amount. Otherwise, it will swap the dragged slot with the target slot.
    /// </summary>
    /// <param name="targetPos">The calculated drop target position.</param>
    /// <param name="amount">The number of items to drop.</param>
    private void DropSomeItems(Vector2Int targetPos, int amount)
    {
        if (!backendInventory.TransferFromDraggedToSlot(targetPos, amount))
        {
            backendInventory.SwapDraggedSlot(targetPos);
        }
    }

    /// <summary>
    /// Drops a specified fraction of the dragged stack into the target slot. If the target slot is empty
    /// or has the same item type and enough space, it will transfer the calculated amount. Otherwise, it will swap the dragged slot with the target slot.
    /// </summary>
    /// <param name="factor">The fraction of the dragged stack to drop (default is 0.5 for half).</param>
    /// <param name="targetPos">The calculated drop target position.</param>
    private void DropPartialStack(Vector2Int targetPos, float factor = 0.5f)
    {
        Slot slot = backendInventory.DraggedSlot;
        if (slot == null || slot.IsEmpty()) return;

        int partialAmount = Mathf.CeilToInt(slot.CurrentAmount * factor);
        bool success = backendInventory.TransferFromDraggedToSlot(targetPos, partialAmount);

        if (!success)
        {
            backendInventory.SwapDraggedSlot(targetPos);
        }
    }

    /// <summary>
    /// Unregisters the global tracking events.
    /// </summary>
    private void StopDragging()
    {
        isDragging = false;
        target.panel.visualTree.UnregisterCallback<PointerMoveEvent>(OnGlobalPointerMove);
        target.panel.visualTree.UnregisterCallback<PointerDownEvent>(OnGlobalPointerDown, TrickleDown.TrickleDown);
    }

    /// <summary>
    /// Updates the absolute position of the global cursor frame to center it on the pointer.
    /// </summary>
    /// <param name="pointerPosition">The current pointer position in screen coordinates.</param>
    private void UpdateCursorPosition(Vector2 pointerPosition)
    {
        if (InventoryUI.CursorFrame == null) return;

        InventoryUI.CursorFrame.style.left = pointerPosition.x - (InventoryUI.CursorFrame.layout.width / 2);
        InventoryUI.CursorFrame.style.top = pointerPosition.y - (InventoryUI.CursorFrame.layout.height / 2);
    }

    /// <summary>
    /// Traverses the visual tree upwards to find the closest parent element classified as an item slot.
    /// </summary>
    /// <param name="element">The starting visual element.</param>
    /// <returns>The parent slot element, or null if not found.</returns>
    private VisualElement FindParentSlot(VisualElement element)
    {
        VisualElement current = element;
        while (current != null)
        {
            if (current.ClassListContains("itemSlot")) return current;
            current = current.parent;
        }
        return null;
    }

    /// <summary>
    /// Finds the closest slot to the given pointer position within the maximum interaction distance.
    /// </summary>
    /// <param name="pointerPosition">The current pointer position.</param>
    /// <returns>The closest slot element, or null if none are within range.</returns>
    private VisualElement FindClosestSlot(Vector2 pointerPosition)
    {
        VisualElement closestSlot = null;
        float closestDistance = float.MaxValue;

        target.panel.visualTree.Query<VisualElement>(className: "itemSlot").ForEach(slot =>
        {
            Vector2 slotPosition = slot.worldBound.center;
            float distance = Vector2.Distance(pointerPosition, slotPosition);

            if (distance < closestDistance && distance <= maxInteractionDistance)
            {
                closestDistance = distance;
                closestSlot = slot;
            }
        });

        return closestSlot;
    }
}