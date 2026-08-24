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

    private List<SlotActionBinding<PointerDownEvent>> slotDownActionBindings = new List<SlotActionBinding<PointerDownEvent>>();
    private List<SlotActionBinding<PointerUpEvent>> slotUpActionBindings = new List<SlotActionBinding<PointerUpEvent>>();
    private List<SlotActionBinding<PointerMoveEvent>> slotMoveActionBindings = new List<SlotActionBinding<PointerMoveEvent>>();
    private static bool isDragging = false;

    /// <summary>
    /// Initializes a new instance of the ItemManipulator class.
    /// </summary>
    /// <param name="inventory">The backend inventory reference.</param>
    /// <param name="slotActionBindings">The list of slot action bindings.</param>
    public ItemManipulator(Inventory inventory,
        List<SlotActionBinding<PointerDownEvent>> slotDownActionBindings,
        List<SlotActionBinding<PointerUpEvent>> slotUpActionBindings,
        List<SlotActionBinding<PointerMoveEvent>> slotMoveActionBindings)
    {
        this.backendInventory = inventory;
        this.slotDownActionBindings = slotDownActionBindings;
        this.slotUpActionBindings = slotUpActionBindings;
        this.slotMoveActionBindings = slotMoveActionBindings;
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
                binding.actionToExecute?.Invoke(backendInventory, (Vector2Int)originalSlot.dataSource, target.panel.Pick(evt.position), evt);
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
                binding.actionToExecute?.Invoke(backendInventory, (Vector2Int)originalSlot.dataSource, target.panel.Pick(evt.position), evt);
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
                binding.actionToExecute?.Invoke(backendInventory, (Vector2Int)originalSlot.dataSource, target.panel.Pick(evt.position), evt);
            }
        } 
   }
}