using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

namespace Kosha82.InventorySystem
{
    /// <summary>
    /// Handles pointer interactions for dragging and dropping items within the inventory UI without capturing the pointer.
    /// </summary>
    public class ItemManipulator : PointerManipulator
    {

        private string slotClassName;
        private float timeWhenPointerDown;

        private List<SlotDownBinding> slotDownActionBindings = new List<SlotDownBinding>();
        private List<SlotUpBinding> slotUpActionBindings = new List<SlotUpBinding>();
        private List<SlotMoveBinding> slotMoveActionBindings = new List<SlotMoveBinding>();
        public static bool isDragging = false;

        /// <summary>
        /// Initializes a new instance of the ItemManipulator class.
        /// </summary>
        /// <param name="inventory">The backend inventory reference.</param>
        /// <param name="slotActionBindings">The list of slot action bindings.</param>
        public ItemManipulator(
            List<SlotDownBinding> slotDownActionBindings,
            List<SlotUpBinding> slotUpActionBindings,
            List<SlotMoveBinding> slotMoveActionBindings,
            string slotClassName)
        {
            this.slotDownActionBindings = slotDownActionBindings;
            this.slotUpActionBindings = slotUpActionBindings;
            this.slotMoveActionBindings = slotMoveActionBindings;
            this.slotClassName = slotClassName;
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
            bool executed = false;
            bool isDraggingTemp = isDragging; // Store the current dragging state for the execution order not to affect the action bindings.

            float timeBeingPressed = (Time.unscaledTime - timeWhenPointerDown);

            for (int i = 0; i < slotUpActionBindings.Count; i++)
            {
                var binding = slotUpActionBindings[i];
                if (binding.requiredButton == (MouseButton)evt.button &&
                    (binding.requiredModifier == EventModifiers.None ||
                     (evt.modifiers & binding.requiredModifier) == binding.requiredModifier)
                      && (binding.whileDragging == isDraggingTemp))
                {
                    binding.action?.Invoke(new InventoryInputUpEventInfo(evt,
                     timeBeingPressed,
                     new InventoryInputEventInfo(
                        GetTargetSlot(target.panel.Pick(evt.position)),
                        target,
                        slotClassName)));
                    executed = true;
                }
            }

            if (executed)
            {
                evt.StopPropagation();
            }

            UpdateGlobalSubscriptions(isDraggingTemp);
        }

        /// <summary>
        /// Handles the pointer down event to initiate dragging and execute any bound actions.
        /// </summary>
        /// <param name="evt">The pointer down event data.</param>
        private void OnPointerDown(PointerDownEvent evt)
        {
            bool executed = false;
            bool isDraggingTemp = isDragging; // Store the current dragging state for the execution order not to affect the action bindings.
            timeWhenPointerDown = Time.unscaledTime; // Record the time when the pointer is pressed down.

            for (int i = 0; i < slotDownActionBindings.Count; i++)
            {
                var binding = slotDownActionBindings[i];
                if (binding.requiredButton == (MouseButton)evt.button &&
                    (binding.requiredModifier == EventModifiers.None ||
                     (evt.modifiers & binding.requiredModifier) == binding.requiredModifier)
                      && (binding.whileDragging == isDraggingTemp))
                {
                    binding.action?.Invoke(new InventoryInputDownEventInfo(evt,
                     new InventoryInputEventInfo(
                        GetTargetSlot(target.panel.Pick(evt.position)),
                        target,
                        slotClassName)));
                    executed = true;
                }
            }

            if (executed)
            {
                evt.StopPropagation();
            }

            UpdateGlobalSubscriptions(isDraggingTemp);
        }


        /// <summary>
        /// Handles the pointer move event to execute any bound actions while dragging or hovering over slots.  
        /// </summary>
        /// <param name="evt"></param>
        private void OnPointerMove(PointerMoveEvent evt)
        {
            bool isDraggingTemp = isDragging; // Store the current dragging state for the execution order not to affect the action bindings.
            float timeBeingPressed = (Time.unscaledTime - timeWhenPointerDown);

            for (int i = 0; i < slotMoveActionBindings.Count; i++)
            {
                var binding = slotMoveActionBindings[i];
                if ((binding.requiredModifier == EventModifiers.None ||
                     (evt.modifiers & binding.requiredModifier) == binding.requiredModifier)
                      && (binding.whileDragging == isDraggingTemp))
                {
                    binding.action?.Invoke(new InventoryInputMoveEventInfo(evt,
                     timeBeingPressed,
                     new InventoryInputEventInfo(
                        GetTargetSlot(target.panel.Pick(evt.position)),
                        target,
                        slotClassName)));
                }
            }
            UpdateGlobalSubscriptions(isDraggingTemp);
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
                if (current.ClassListContains(slotClassName) && current.dataSource is SlotDirection)
                {
                    return current;
                }
                current = current.parent;
            }

            return null;
        }

        private void UpdateGlobalSubscriptions(bool wasDragging)
        {
            if (!wasDragging && isDragging)
            {
                target.panel.visualTree.RegisterCallback<PointerMoveEvent>(OnPointerMove);
                target.panel.visualTree.RegisterCallback<PointerDownEvent>(OnPointerDown);
                target.panel.visualTree.RegisterCallback<PointerUpEvent>(OnPointerUp);
            }
            else if (wasDragging && !isDragging)
            {
                target.panel.visualTree.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
                target.panel.visualTree.UnregisterCallback<PointerDownEvent>(OnPointerDown);
                target.panel.visualTree.UnregisterCallback<PointerUpEvent>(OnPointerUp);
            }
        }
    }
}