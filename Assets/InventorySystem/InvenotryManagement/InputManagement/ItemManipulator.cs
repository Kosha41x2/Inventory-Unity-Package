using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

/// <summary>
/// Handles pointer interactions for dragging and dropping items within the inventory UI without capturing the pointer.
/// </summary>
public class ItemManipulator : PointerManipulator
{
    private VisualElement originalSlot;
    
    private Inventory backendInventory;

    private string slotClassName;

    private List<SlotDownBinding> slotDownActionBindings = new List<SlotDownBinding>();
    private List<SlotUpBinding> slotUpActionBindings = new List<SlotUpBinding>();
    private List<SlotMoveBinding> slotMoveActionBindings = new List<SlotMoveBinding>();
    public static bool isDragging = false;

    /// <summary>
    /// Initializes a new instance of the ItemManipulator class.
    /// </summary>
    /// <param name="inventory">The backend inventory reference.</param>
    /// <param name="slotActionBindings">The list of slot action bindings.</param>
    public ItemManipulator(Inventory inventory,
        List<SlotDownBinding> slotDownActionBindings,
        List<SlotUpBinding> slotUpActionBindings,
        List<SlotMoveBinding> slotMoveActionBindings,
        string slotCassName)
    {
        this.backendInventory = inventory;
        this.slotDownActionBindings = slotDownActionBindings;
        this.slotUpActionBindings = slotUpActionBindings;
        this.slotMoveActionBindings = slotMoveActionBindings;
        this.slotClassName = slotCassName;
    }
    
    /// <summary>
    /// Registers the local pointer down event to start dragging.
    /// </summary>
    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<PointerDownEvent>(OnPointerDown);
        target.RegisterCallback<PointerUpEvent>(OnPointerUp);
        target.RegisterCallback<PointerMoveEvent>(OnPointerMove);
    }

    /// <summary>
    /// Unregisters local callbacks and ensures global callbacks are cleaned up to prevent memory leaks.
    /// </summary>
    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
        target.UnregisterCallback<PointerUpEvent>(OnPointerUp);
        target.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
    }

    /// <summary>
    /// Handles the pointer up event to stop dragging and execute any bound actions.
    /// </summary>
    /// <param name="evt"></param>
    private void OnPointerUp(PointerUpEvent evt)
    {
        for(int i = 0; i < slotUpActionBindings.Count; i++)
        {
            var binding = slotUpActionBindings[i];
            if (binding.requiredButton == (MouseButton)evt.button && 
                (binding.requiredModifier == EventModifiers.None || 
                 (evt.modifiers & binding.requiredModifier) == binding.requiredModifier)
                  && (binding.whileDragging == isDragging))
            {
                binding.action?.Invoke(backendInventory, GetTargetSlot(target.panel.Pick(evt.position)), evt);
            }
        }
    }

    /// <summary>
    /// Handles the pointer down event to initiate dragging and execute any bound actions.
    /// </summary>
    /// <param name="evt">The pointer down event data.</param>
    private void OnPointerDown(PointerDownEvent evt)
    {
        for(int i = 0; i < slotDownActionBindings.Count; i++)
        {
            var binding = slotDownActionBindings[i];
            if (binding.requiredButton == (MouseButton)evt.button && 
                (binding.requiredModifier == EventModifiers.None || 
                 (evt.modifiers & binding.requiredModifier) == binding.requiredModifier)
                  && (binding.whileDragging == isDragging))
            {
                binding.action?.Invoke(backendInventory, GetTargetSlot(target.panel.Pick(evt.position)), evt);
            }
        } 
   }


    /// <summary>
    /// Handles the pointer move event to execute any bound actions while dragging or hovering over slots.  
    /// </summary>
    /// <param name="evt"></param>
   private void OnPointerMove(PointerMoveEvent evt)
   {
        for(int i = 0; i < slotMoveActionBindings.Count; i++)
        {
            var binding = slotMoveActionBindings[i];
            if ( (binding.requiredModifier == EventModifiers.None || 
                 (evt.modifiers & binding.requiredModifier) == binding.requiredModifier)
                  && (binding.whileDragging == isDragging))
            {
                binding.action?.Invoke(backendInventory, GetTargetSlot(target.panel.Pick(evt.position)), evt);
            }
        } 
    }

    /// <summary>
    /// Finds the target slot visual element based on the element under the pointer. It traverses up the visual tree to find a parent element that matches the slot class name and has a valid data source.
    /// </summary>
    /// <param name="elementUnderPointer"></param>
    /// <returns></returns>
    private VisualElement GetTargetSlot(VisualElement elementUnderPointer)
    {
        VisualElement current = elementUnderPointer;
        while (current != null)
        {
            if (current.ClassListContains(slotClassName) && current.dataSource is Vector2Int pos)
            {
                return current;
            }
            current = current.parent;
        }
        return null;
    }
}