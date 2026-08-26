using UnityEngine;

[System.Serializable]
public class UIDocumentSettings
{
    [Tooltip("The name of the inventory panel in the UI Document, it might be a VisualElement or a ScrollView, but it must be the desired container for the inventory slots (Check given Assets/UI/InventoryUI.uxml for reference)")]
    public string inventoryPanelName = "Inventory";

    [Space(5)] 
    
    [Tooltip("The name of the class that will be used for the slot sub-containers in the inventory UI (meaning an auxiliary container in the inventory panel for each row/column of slots), this class should be defined in the UI Document's USS file (Check given Assets/UI/USS/InventoryUI.uss for reference)")]
    public string slotSubContainerClassName = "slotRow";

    [Tooltip("The name of the class that will be used for the item slots in the inventory UI (the containers for individual items), this class should be defined in the UI Document's USS file (Check given Assets/UI/USS/InventoryUI.uss for reference)")]
    public string itemSlotClassName = "itemSlot";

    [Tooltip("The name of the class that will be used for the item frames in the inventory UI (each slot has a container and they display the item icon), this class should be defined in the UI Document's USS file (Check given Assets/UI/USS/InventoryUI.uss for reference)")]
    public string itemFrameClassName = "itemFrame";

    [Tooltip("The name of the class that will be used for the stack size labels in the inventory UI (the labels that display the number of items in each slot), this class should be defined in the UI Document's USS file (Check given Assets/UI/USS/InventoryUI.uss for reference)")]
    public string stackSizeLabelClassName = "stackSize";
}