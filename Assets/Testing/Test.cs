using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using Kosha82.InventorySystem;
using Kosha82.InventorySystem.Examples;

public class Test: MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private List<Item> itemToAdd;

    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Debug.Log("Adding items to inventory...");
            foreach (var item in itemToAdd)
            {
                inventory.AddItemToInventory(item, 1);
                Debug.Log(inventory.GetSlot(0, 0).CurrentItem?.ItemName);
            }
        }

        if(Keyboard.current.numpadPlusKey.wasPressedThisFrame)
        {
            inventory.GetSlot(0, 0).CurrentItem.getComponent<ItemDamage>().IncreaseDamage(1);
        }
    }
}
