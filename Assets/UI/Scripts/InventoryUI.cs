using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;

[ExecuteAlways]
[AddComponentMenu("Inventory System/UI/Inventory UI")]
public class InventoryUI : MonoBehaviour
{
    private VisualElement root;
    private VisualElement inventoryPanel;

    private static VisualElement cursorFrame;
    private static Label cursorStackLabel;

    [Header("Backend Connections")]
    [Tooltip("Drag an object with an Inventory component to this field. This will make this UI display the inventory of that object")]
    [SerializeField] private Inventory inventory;

    [Header("UI Toolkit Settings")]
    [SerializeField] private UIDocumentSettings uiSettings;

    [Header("Slot Down Action Bindings")]
    [Tooltip("A list of actions that will be executed when a slot is clicked or pressed down. You can configure the mouse button and modifier key for each action.")]
    [SerializeField]
    private List<SlotDownBinding> slotDownActionBindings = new List<SlotDownBinding>();

    [Header("Slot Up Action Bindings")]
    [Tooltip("A list of actions that will be executed when a slot is released or pressed up. You can configure the mouse button and modifier key for each action.")]
    [SerializeField]
    private List<SlotUpBinding> slotUpActionBindings = new List<SlotUpBinding>();

    [Header("Slot Move Action Bindings")]
    [Tooltip("A list of actions that will be executed when a slot is hovered or moved over. You can configure the mouse button and modifier key for each action.")]
    [SerializeField]
    private List<SlotMoveBinding> slotMoveActionBindings = new List<SlotMoveBinding>();

    public static VisualElement CursorFrame => cursorFrame;
    public static Label CursorStackLabel => cursorStackLabel;
    void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        if (root == null || uiSettings == null) return;

        inventoryPanel = root.Q<VisualElement>(uiSettings.inventoryPanelName);
        inventory.OnInventorySizeChanged += BuildInventory;
        inventory.OnInventoryContentChanged += UpdateInventoryContent;
        inventory.OnSlotContentChanged += UpdateSlot;
        inventory.OnDraggedSlotContentChanged += UpdateDraggedSlotVisuals;
    }

    void OnDisable()
    {
        inventory.OnInventorySizeChanged -= BuildInventory;
        inventory.OnInventoryContentChanged -= UpdateInventoryContent;
        inventory.OnSlotContentChanged -= UpdateSlot;
        inventory.OnDraggedSlotContentChanged -= UpdateDraggedSlotVisuals;
    }

    void SetInventory(Inventory newInventory)
    {
        InitializeCursorFrame();

        if (this.inventory != null)
        {
            this.inventory.OnInventorySizeChanged -= BuildInventory;
            this.inventory.OnInventoryContentChanged -= UpdateInventoryContent;
            this.inventory.OnSlotContentChanged -= UpdateSlot;
            this.inventory.OnDraggedSlotContentChanged -= UpdateDraggedSlotVisuals;
        }

        this.inventory = newInventory;
        inventory.OnInventorySizeChanged += BuildInventory;
        inventory.OnInventoryContentChanged += UpdateInventoryContent;
        inventory.OnSlotContentChanged += UpdateSlot;
        inventory.OnDraggedSlotContentChanged += UpdateDraggedSlotVisuals;
    }

/// <summary>
/// This method builds the inventory UI based on the given inventory.
/// </summary>
/// <param name="inventory"></param>
    void BuildInventory(Inventory inventory)
    {
        inventoryPanel.Clear();

        InitializeCursorFrame();

        int rows = inventory.InventoryVerticalSize;
        int columns = inventory.InventoryHorizontalSize;

        for(int y = 0; y < rows; y++)
        {
            string rowName = $"Row {y}";
            VisualElement row = new VisualElement();
            row.name = rowName;
            row.dataSource = y;
            row.AddToClassList(uiSettings.slotSubContainerClassName);
            inventoryPanel.Add(row);

            for(int x = 0; x < columns; x++)
            {
                string slotName = $"Slot {x}_{y}";
                VisualElement slot = new VisualElement();
                slot.name = slotName;
                slot.AddToClassList(uiSettings.itemSlotClassName);
                row.Add(slot);

                string itemFrameName = $"ItemFrame {x}_{y}";
                VisualElement itemFrame = new VisualElement();
                itemFrame.name = itemFrameName;
                itemFrame.AddToClassList(uiSettings.itemFrameClassName);

                itemFrame.AddManipulator(new ItemManipulator(inventory, slotDownActionBindings, slotUpActionBindings, slotMoveActionBindings, uiSettings.itemSlotClassName));

                slot.Add(itemFrame);



                string stackSizeName = $"StackSize {x}_{y}";
                Label stackSizeLabel = new Label();
                stackSizeLabel.name = stackSizeName;
                stackSizeLabel.AddToClassList(uiSettings.stackSizeLabelClassName);
                itemFrame.Add(stackSizeLabel);
            }
        }
    }

    private void InitializeCursorFrame()
    {
        cursorFrame = new VisualElement();
        cursorFrame.name = "CursorFrame";
        cursorFrame.AddToClassList(uiSettings.itemFrameClassName);
        cursorFrame.style.position = Position.Absolute;
        cursorFrame.style.display = DisplayStyle.None;
        cursorFrame.pickingMode = PickingMode.Ignore;

        cursorStackLabel = new Label();
        cursorStackLabel.AddToClassList(uiSettings.stackSizeLabelClassName);
        cursorFrame.Add(cursorStackLabel);

        root.Add(cursorFrame);
    }

    private void UpdateDraggedSlotVisuals(Inventory inv)
    {
        if (cursorFrame == null || cursorStackLabel == null)
        {
            Debug.LogWarning("Cursor frame or stack label is not initialized. Initializing now.");
            InitializeCursorFrame();
            
            if (cursorFrame == null) return; 
        }

        Slot draggedSlot = inv.DraggedSlot;

        if (draggedSlot == null || draggedSlot.IsEmpty() || draggedSlot.CurrentItem == null)
        {
            cursorFrame.style.display = DisplayStyle.None;
            cursorFrame.style.backgroundImage = new StyleBackground();
            cursorStackLabel.text = string.Empty;
        }
        else
        {
            cursorFrame.style.display = DisplayStyle.Flex;
            cursorFrame.style.backgroundImage = new StyleBackground(draggedSlot.CurrentItem.ItemIcon);
            cursorStackLabel.text = draggedSlot.CurrentAmount.ToString();
        }
    }

    /// <summary>
    /// This method updates the inventory UI content based on the given inventory.
    /// It sets the background image of each item frame based on the item in the corresponding slot
    /// </summary>
    /// <param name="inventory"></param>
    private void UpdateInventoryContent(Inventory inventory)
    {
        int rows = inventory.InventoryVerticalSize;
        int columns = inventory.InventoryHorizontalSize;

        for(int y = 0; y < rows; y++)
        {
            for(int x = 0; x < columns; x++)
            {
                UpdateSlot(inventory, new Vector2Int(x, y));
            }
        }
    }

    /// <summary>
    /// This method updates a specific slot in the inventory UI based on the given inventory and position.
    /// It sets the background image of the item frame based on the item in the corresponding slot
    /// </summary>
    /// <param name="inventory"></param>
    /// <param name="position"></param>
    void UpdateSlot(Inventory inventory, Vector2Int position)
    {
        Slot slotData = inventory.GetSlot(position.x, position.y);
        VisualElement slot = inventoryPanel.Q<VisualElement>($"Slot {position.x}_{position.y}");
        if(slot == null) return;

        slot.dataSource = position;
        VisualElement itemFrame = slot.Q<VisualElement>($"ItemFrame {position.x}_{position.y}");
        Label stackSizeLabel = itemFrame.Q<Label>($"StackSize {position.x}_{position.y}");

        if(itemFrame == null) 
            {Debug.LogWarning($"ItemFrame {position.x}_{position.y} not found.");
            return;
        }

        if(stackSizeLabel == null) 
            {Debug.LogWarning($"StackSize {position.x}_{position.y} not found.");
            return;
        }

        UpdateFrame(slotData, itemFrame, stackSizeLabel);
    }

    /// <summary>
    /// Updates the visual representation of a slot based on the provided slot data, item frame, and stack size label. If the slot is empty, it clears the background image and stack size label. If the slot contains an item, it sets the background image to the item's icon and updates the stack size label with the current amount of items in the slot.
    /// </summary>
    /// <param name="slotData"></param>
    /// <param name="itemFrame"></param>
    /// <param name="stackSizeLabel"></param>
    private void UpdateFrame(Slot slotData, VisualElement itemFrame, Label stackSizeLabel = null)
    {
        if (stackSizeLabel == null)
        {
            stackSizeLabel = new Label();
            stackSizeLabel.AddToClassList(uiSettings.stackSizeLabelClassName);
            stackSizeLabel.name = $"StackSize {itemFrame.name}";
            itemFrame.Add(stackSizeLabel);
        }

        if(slotData.IsEmpty())
        {
            itemFrame.style.backgroundImage = new StyleBackground();
            stackSizeLabel.text = string.Empty;
        }
        else
        {
            itemFrame.style.backgroundImage = new StyleBackground(slotData.CurrentItem.ItemIcon);
            stackSizeLabel.text = slotData.CurrentAmount.ToString();
        }
    }

    /// <summary>
    /// Adjusts the size of the cursor frame to match the size of the provided model visual element. This is typically used to ensure that the cursor frame visually matches the item being dragged or interacted with in the inventory UI.
    /// Usually necessary when getting an item from a slot and dragging it, so the cursor frame matches the size of the item frame.
    /// </summary>
    /// <param name="model"></param>
    public static void AdjustCursorFrame(VisualElement model)
    {
        cursorFrame.style.width = model.resolvedStyle.width;
        cursorFrame.style.height = model.resolvedStyle.height;
    }

    /// <summary>
    /// Updates the position of the cursor frame to follow the mouse cursor. The cursor frame is
    /// centered on the cursor position, and its position is adjusted based on its width and height to ensure it remains centered.
    /// </summary>
    /// <param name="position"></param>
    public static void UpdateCursorFramePosition(Vector2 position)
    {
        if (cursorFrame == null) return;

        cursorFrame.style.left = position.x - cursorFrame.resolvedStyle.width / 2;
        cursorFrame.style.top = position.y - cursorFrame.resolvedStyle.height / 2;
    }
}
