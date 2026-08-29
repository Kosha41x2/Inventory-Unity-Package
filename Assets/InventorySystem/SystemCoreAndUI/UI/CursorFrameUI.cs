using UnityEngine;
using UnityEngine.UIElements;

namespace Kosha82.InventorySystem
{
    public static class CursorFrameUI
    {
        public static VisualElement CursorFrame { get; private set; }
        public static Label CursorFrameLabel { get; private set; }

        internal static void InitializeCursorFrame(VisualElement root, UIDocumentSettings uiSettings)
        {
            CursorFrame = new VisualElement();
            CursorFrame.name = "CursorFrame";
            CursorFrame.AddToClassList(uiSettings.itemFrameClassName);
            CursorFrame.style.position = Position.Absolute;
            CursorFrame.style.display = DisplayStyle.None;
            CursorFrame.pickingMode = PickingMode.Ignore;

            CursorFrameLabel = new Label();
            CursorFrameLabel.AddToClassList(uiSettings.stackSizeLabelClassName);
            CursorFrame.Add(CursorFrameLabel);

            root.Add(CursorFrame);
        }

        /// <summary>
        /// Adjusts the size of the cursor frame to match the size of the provided model visual element. This is typically used to ensure that the cursor frame visually matches the item being dragged or interacted with in the inventory UI.
        /// Usually necessary when getting an item from a slot and dragging it, so the cursor frame matches the size of the item frame.
        /// </summary>
        /// <param name="model"></param>
        public static void AdjustSizeToMatch(VisualElement model)
        {
            if (CursorFrame == null) return;
            
            CursorFrame.style.width = model.resolvedStyle.width;
            CursorFrame.style.height = model.resolvedStyle.height;
            CursorFrame.style.display = DisplayStyle.Flex;
        }

        /// <summary>
        /// Updates the position of the cursor frame to follow the mouse cursor. The cursor frame is
        /// centered on the cursor position, and its position is adjusted based on its width and height to ensure it remains centered.
        /// </summary>
        /// <param name="position"></param>
        public static void UpdatePosition(Vector2 position)
        {
            if (CursorFrame == null) return;

            CursorFrame.style.left = position.x - CursorFrame.resolvedStyle.width / 2;
            CursorFrame.style.top = position.y - CursorFrame.resolvedStyle.height / 2;
        }

        /// <summary>
        /// Hides the cursor frame.
        /// </summary>
        public static void Hide()
        {
            if (CursorFrame != null) CursorFrame.style.display = DisplayStyle.None;
        }
    }
}