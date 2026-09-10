# Kosha82 Inventory System V1

# Kosha82 Inventory System V1

A Unity package made to handle anything related to inventory management and items. 
This package may be used to create inventories, stores, loot, etc.

## Architecture overview

This system's main core can be divided into three main layers:

**Data Layer**: This is what is stored in the `SystemCoreAndUI/ItemSystem` folder. This system could be used independently from the rest of the inventory system and it focuses on using `ScriptableObjects` to create items and item components. 
When creating an item you can add item components to it. `ItemComponent` is a class all item components must inherit from and it is a `ScriptableObject`. Once you create one, you can add different interfaces to them. If you decide to add `IItemDynamicComponent` to it, any item with it will behave as a dynamic item, meaning that different instances of the item will be created and destroyed when needed in the inventory; otherwise, it'll behave just as a single defined `ScriptableObject`. Dynamic components need to be used for any component that might store data that may change through gameplay.

**Logical and organization layer**: In this layer we use classes like `Slot` or `Inventory` to manage the items. The `Slot` class methods are mostly internal, because `Inventory` is supposed to take the burden to move items around and update everything. `Slot` stores both an item and an amount, while `Inventory` is just a matrix of slots with different methods to swap and move items from one to the other. There's also a static slot, which is the `draggedSlot` inside `Inventory`.

**Presentation and input layer**: This is one of the most complicated layers, and it'll be explained in greater detail later. Mainly we have an `InventoryUI` class that will connect to an `Inventory`. It must be assigned to a `UIDocument` from the UI Toolkit, and it creates the visual inventory automatically into the desired `VisualElement`. It uses strings to make use of the class styles from `USS`. It also handles the input automatically with `ItemManipulator`, with each `Slot` having one of them assigned. When needed, it also updates the slot visuals automatically through a chain of events.

---

## Creating an item

This system's item creation is highly modular, and there are examples of items to use in `Examples/ItemSystem`.

**Create the item itself**: Just right-click into the inspector and go to `Create/InventorySystem/Item`, then assign a name, sprite, stack size, and most importantly, an ID. 
`Item ID` as a field might be easy to forget, but it is really important for all items to have a different ID. Otherwise, different items might start stacking even if they're different. 
Then you can leave it as it is, but you will likely want to add some components to your item.

**Create a type of component**: Create a new class that inherits from `ItemComponent` and use this exact line on top of the class definition, changing `MyComponent` to the name you wish your component has in the inspector:

```csharp
[CreateAssetMenu(fileName = "MyComponent", menuName = "Inventory System/Item Components/MyComponent")]
```

## Creating an inventory

This package adds 2 new components to be added to your `gameObjects`.

**Inventory Component**: Go to your `gameObject` and go to `Add Component`, then `Inventory System/Core/Backend Inventory`. This component just assigns an instance of the class `Inventory` to your object, which is just a way to store the items and quantities. It works on its own, and if the inventory is not meant to be visible (or it rather represents the loot of an enemy's inventory), you can just leave it like that. You can modify the number of rows and columns. You can use the methods inside freely, and if you have an `InventoryUI` (we will talk about it later), it will manage the updates on its own.

**Inventory UI**: Go to your `UIDocument` (`UIToolkit` required, it's included in this package) and go to `Add Component`, then `Inventory System/UI/Inventory UI`. This is a much more complicated component. The first thing you have to do is assign it a `Backend Inventory`; just drag your game object to the `Inventory` slot in this component. Once you've done that, you have to set an `Inventory Panel`. It can be any visual component, but you can see an example of a completely built Visual Tree and USS in `Examples/UI Template`. After setting the panel, you also have to set the style class names; you also have an example of them in `Examples/UI Template`.

**Style Clsses**
To summarize, `Slot Subcontainer Class Name` in this example is just a style class for the rows, but it could be modified to represent the columns. This one doesn't have much to it; it's just an empty visual element that is used to organize the slots better. Still, the class has to be created in case you are not using the example, which is heavily recommended to check first.
Then `Item Slot Class Name` is just the visuals for the slot. In the example, the slots are shown in a darker color than the rest of the panel.
`Item Frame Class Name` is just where the item is shown. It is also the visual part that takes the input. It is always there even if there's no item, but the image will be that of the held item. It is nested inside the slots.
`Stack Size Label` is just the style for the number that will show the current amount of items in the slot. It is nested in the `Item Frame`.

Then we have the `Component Style Mappings`. If we have created a showable component, regardless of it being an image or text, it must return a tag. That tag will be mapped here with a style class name. Then this class will automatically show the component image or text in the defined style. If you have a showable component, but you don't map the set tag to a defined class here, it won't be shown. You can take advantage of this to show some attributes only in certain inventory UIs.

**Input**
Lastly, we have `Slot Action Bindings`. The `Down` one is for when you click down on an item, the `Move` one is for when you hover over a slot, and the `Up` one is for when you lift the down button on a slot. Using a `PointerManipulator`, each frame has its own `ItemManipulator` that manages all these inputs automatically so that the user doesn't have to worry about it. You just have to add a binding to your desired action (up, down, or move), then select the click that will cause the interaction (in case of move, none), and a modifying key if any. `While Dragging` just marks whether the static field of `ItemManipulator` `isDragging` is active or not. You can turn it on or off. You should turn it on if your action will start a dragging or transporting item process, and off if you just finished that action; checking `Examples/CustomActions` is recommended. When you start dragging, always call:
```csharp
 CursorFrameUI.AdjustSizeToMatch(slotElement);
```
being `slotElement` the clicked slot. This is necessary for the `CursorFrameUI` to be correctly visible.
And then select the desired action (you can define the action in any script and just drag it). In `Examples/CustomActions` you have some actions you can add.

The action parameters: these actions must take only one parameter: `InventoryInputDownEventInfo`, `InventoryInputUpEventInfo`, or `InventoryInputMoveEventInfo`. It is clear which one you will have to use for each thing. These contain information about what visual slot was clicked, the target processing the input, how long it's been since the button was pressed down, etc.
In the visual slot data source, there's a `SlotDirection` that tells us in which inventory and position of the matrix the slot is.