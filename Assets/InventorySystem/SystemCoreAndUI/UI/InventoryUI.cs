using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

namespace Kosha82.InventorySystem
{
    [ExecuteAlways]
    [AddComponentMenu("Inventory System/UI/Inventory UI")]
    public class InventoryUI : MonoBehaviour
    {
        private VisualElement root;
        private VisualElement inventoryPanel;

        private Dictionary<string, string> componentStyleMappingDict = new Dictionary<string, string>();

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

        void OnEnable()
        {
            root = GetComponent<UIDocument>().rootVisualElement;

            if (root == null || uiSettings == null || inventory == null) return;

            inventoryPanel = root.Q<VisualElement>(uiSettings.inventoryPanelName);
            inventory.OnInventorySizeChanged += BuildInventory;
            inventory.OnInventoryContentChanged += UpdateInventoryContent;
            inventory.OnSlotContentChanged += UpdateSlot;
            inventory.OnDraggedSlotContentChanged += UpdateDraggedSlotVisuals;

            BuildInventory(inventory);
            CursorFrameUI.InitializeCursorFrame(root, uiSettings);
            InitializeComponentStyleMappingDict();
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

            int rows = inventory.InventoryVerticalSize;
            int columns = inventory.InventoryHorizontalSize;

            for (int y = 0; y < rows; y++)
            {
                string rowName = $"Row {y}";
                VisualElement row = new VisualElement();
                row.name = rowName;
                row.dataSource = y;
                row.AddToClassList(uiSettings.slotSubContainerClassName);
                inventoryPanel.Add(row);

                for (int x = 0; x < columns; x++)
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

                    itemFrame.AddManipulator(new ItemManipulator(slotDownActionBindings, slotUpActionBindings, slotMoveActionBindings, uiSettings.itemSlotClassName));

                    slot.Add(itemFrame);



                    string stackSizeName = $"StackSize {x}_{y}";
                    Label stackSizeLabel = new Label();
                    stackSizeLabel.name = stackSizeName;
                    stackSizeLabel.AddToClassList(uiSettings.stackSizeLabelClassName);
                    itemFrame.Add(stackSizeLabel);
                }
            }
        }


        /// <summary>
        /// Initializes the component style mapping dictionary based on the provided component style mappings in the UI settings. This dictionary is used to map component tags to their corresponding style classes for customizing the appearance of item components in the inventory UI. If duplicate tags are found, a warning is logged, and only the first occurrence is used.
        /// </summary>
        private void InitializeComponentStyleMappingDict()
        {
            componentStyleMappingDict.Clear();
            foreach (var mapping in uiSettings.componentStyleMappings)
            {
                if (mapping == null) continue;
                if (!componentStyleMappingDict.ContainsKey(mapping.tag))
                {
                    componentStyleMappingDict.Add(mapping.tag, mapping.styleClass);
                }
                else
                {
                    Debug.LogWarning($"Duplicate tag '{mapping.tag}' found in component style mappings. Only the first occurrence will be used.");
                }
            }
        }
        private void UpdateDraggedSlotVisuals()
        {
            UpdateFrame(Inventory.DraggedSlot, CursorFrameUI.CursorFrame, CursorFrameUI.CursorFrameLabel);
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

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
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
            if (slot == null) return;

            slot.dataSource = new SlotDirection(position, inventory);
            VisualElement itemFrame = slot.Q<VisualElement>($"ItemFrame {position.x}_{position.y}");
            Label stackSizeLabel = itemFrame.Q<Label>($"StackSize {position.x}_{position.y}");

            if (itemFrame == null)
            {
                Debug.LogWarning($"ItemFrame {position.x}_{position.y} not found.");
                return;
            }

            if (stackSizeLabel == null)
            {
                Debug.LogWarning($"StackSize {position.x}_{position.y} not found.");
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

            ResetComponentStyles(itemFrame);

            if (slotData.IsEmpty())
            {
                itemFrame.style.backgroundImage = new StyleBackground();
                stackSizeLabel.text = string.Empty;
            }
            else
            {
                itemFrame.style.backgroundImage = new StyleBackground(slotData.CurrentItem.ItemIcon);
                stackSizeLabel.text = slotData.CurrentAmount.ToString();
            }

            UpdateComponentStyles(slotData, itemFrame);
        }

        private void UpdateComponentTextStyles(Slot slotData, VisualElement itemFrame)
        {
            if (itemFrame == null || slotData == null) return;
            if (slotData.IsEmpty()) return;

            slotData.CurrentItem.GetDisplayableTextComponents().ForEach(component =>
            {
                if (componentStyleMappingDict.TryGetValue(component.GetInInventoryDisplayTag(), out string styleClass))
                {
                    Label label = itemFrame.Q<Label>(className: styleClass);
                    if (label == null)
                    {
                        label = CreateComponentLabel(itemFrame, styleClass);
                    }
                    label.visible = true;
                    label.text = component.GetInInventoryDisplayText();
                }
            });
        }


        private Label CreateComponentLabel(VisualElement itemFrame, string styleClass)
        {
            Label label = new Label();
            label.AddToClassList(styleClass);
            label.name = $"Label {itemFrame.name} {styleClass}";
            label.style.position = Position.Absolute;

            itemFrame.Add(label);
            return label;
        }

        private void UpdateComponentImageStyles(Slot slotData, VisualElement itemFrame)
        {
            if (itemFrame == null || slotData == null) return;
            if (slotData.IsEmpty()) return;

            slotData.CurrentItem.GetDisplayableImageComponents().ForEach(component =>
            {
                if (componentStyleMappingDict.TryGetValue(component.GetInInventoryDisplayTag(), out string styleClass))
                {
                    VisualElement imageElement = itemFrame.Q<VisualElement>(className: styleClass);
                    if (imageElement == null)
                    {
                        imageElement = CreateComponentImage(itemFrame, styleClass);
                    }
                    imageElement.visible = true;
                    imageElement.style.backgroundImage = new StyleBackground(component.GetInInventoryDisplayImage());
                }
            });
        }

        private VisualElement CreateComponentImage(VisualElement itemFrame, string styleClass)
        {
            VisualElement imageElement = new VisualElement();
            imageElement.AddToClassList(styleClass);
            imageElement.name = $"Image {itemFrame.name} {styleClass}";
            imageElement.style.position = Position.Absolute;

            itemFrame.Add(imageElement);
            return imageElement;
        }

        private void UpdateComponentStyles(Slot slotData, VisualElement itemFrame)
        {
            UpdateComponentTextStyles(slotData, itemFrame);
            UpdateComponentImageStyles(slotData, itemFrame);
        }

        private void ResetComponentTextStyles(VisualElement itemFrame)
        {
            if (itemFrame == null) return;

            foreach (var mapping in componentStyleMappingDict)
            {
                itemFrame.Query<Label>(className: mapping.Value).ForEach(label =>
                {
                    if (label == null) return;
                    label.visible = false;
                    label.text = string.Empty;
                });
            }
        }

        private void ResetComponentImageStyles(VisualElement itemFrame)
        {
            if (itemFrame == null) return;

            foreach (var mapping in componentStyleMappingDict)
            {
                itemFrame.Query<VisualElement>(className: mapping.Value).ForEach(imageElement =>
                {
                    if (imageElement == null) return;
                    imageElement.visible = false;
                    imageElement.style.backgroundImage = new StyleBackground((Texture2D)null);
                });
            }
        }

        private void ResetComponentStyles(VisualElement itemFrame)
        {
            ResetComponentTextStyles(itemFrame);
            ResetComponentImageStyles(itemFrame);
        }
    }
}