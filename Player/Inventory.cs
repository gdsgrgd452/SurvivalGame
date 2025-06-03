using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Crafting;

public class Inventory : MonoBehaviour
{
    [SerializeField] private ObjectGrab objectInHand; //Object in the players hand
    [System.Serializable] public class InventorySlot { //Object for each slot containing info about it
        public string type;
        public List<ObjectGrab> items;
        public GameObject UI;
        public int count;
        public bool full;
        public Vector2 defaultPos;
    }
    public List<InventorySlot> slots; //A list of the inventory slots
    [SerializeField] private InventorySlot currentSlot; //The currently selected slot
    public int currentSlotIndex; //Index of the currently selected slot
    public Sprite[] objectImages; //Images to display in the UI for each object
    [SerializeField] private Vector2 selectedPos = new(126, -200);
    //Other files that need to know the item in hand
    [SerializeField] private ToolTips toolTips;
    [SerializeField] private PlayerActions playerActions;


    //Loading the inventory

    public void LoadInventory(int currentInvSlot) { //Loads the inventory from the save file
        currentSlotIndex = currentInvSlot; //Sets the current slot to the one in the save file
        currentSlot = slots[currentSlotIndex]; //Sets the current slot to the one in the save file
        SwitchSlot(currentSlotIndex); //Switches to the current slot
    }
  

    //Putting items into the players hand 

    private void NewItemIntoHand(ObjectGrab item) {  //For when the slot is empty and an item is put into it
        objectInHand = item;  //Put the new object into the players hand and enable it
        objectInHand.gameObject.SetActive(true);
        toolTips.objectInHand = objectInHand;
        playerActions.objectInHand = objectInHand;
    }

    private void NextItemFromSlot(InventorySlot slot) { //For when an item is taken out of a slot with more items in or the slot is switched to
        objectInHand = (slot.items.Count > 0) ? slot.items[0] : null;
        Debug.Log($"Next item in hand: {objectInHand?.oType ?? "None"}");
        if (objectInHand == null) { //If there is no items in the slot
            toolTips.objectInHand = null;
            playerActions.objectInHand = null;
            return; //Exit the function
        }
        objectInHand.gameObject.SetActive(true);
        toolTips.objectInHand = objectInHand;
        playerActions.objectInHand = objectInHand;
    }

    private void CurrentSlotEmpty(InventorySlot slot) { //For when the current slot is made empty
        slot.type = ""; //Clear the slot type to allow any type of items in it
        objectInHand = null; //Clear the object in hand
        toolTips.objectInHand = objectInHand;
        playerActions.objectInHand = objectInHand;
        Debug.Log("Hand now empty");
    }
      
    //Adding items

    public bool AddItem(ObjectGrab objectPickedUp) { //Passing in the item you are trying to pick up (Returning true or false (item can be picked up or not))
        string objType = objectPickedUp.oType; //Type of item picked up
        InventorySlot freeSlot = FindEmptyOrSameSlot(objType); //Finds a free slot or one with the same type of items
        if (freeSlot == null) {return false;} //Returns false if there is no free slot found
        if (freeSlot.type == "") {freeSlot.type = objType;} //If an empty slot is found its type is set
        freeSlot.items.Add(objectPickedUp); //Adds the new item to the list in the slot
        freeSlot.count = freeSlot.items.Count; 
        UpdateUi(freeSlot); //Update the UI
        if (freeSlot.count == 10) {freeSlot.full = true;}
        objectPickedUp.gameObject.transform.SetParent(freeSlot.UI.transform); //Sets the picked up object as a child of the UI transform
        objectPickedUp.gameObject.SetActive(false); //Disables the new item
        if (freeSlot == currentSlot && objectInHand == null) { //If there is nothing in the hand and the item goes into the slot the player has selected
            NewItemIntoHand(objectPickedUp);
        }
        return true;
    }

    private InventorySlot FindEmptyOrSameSlot(string objType) { 
        foreach (InventorySlot slot in slots) {
            if (slot.items.Count == 0 || (slot.type == objType && !slot.full)) { //If the slot is empty or the same type and not full
                return slot;
            }
        }
        return null;
    }
    
    //Removing items

    public bool RemoveItem(ObjectGrab objGrab, bool destroy = false) { //Passing in the item you are removing and if you are destroying it or not (e.g eating it)
        if (currentSlot.count <= 0 || currentSlot.items.Count <= 0) {return false;} //If there is no items in the slot return false
        if (currentSlot.count == 10) {currentSlot.full = false;}
        currentSlot.items.Remove(objGrab);
        Debug.Log("Called Remove Function");
        currentSlot.count = currentSlot.items.Count;  //Remove the item from the list of items in the slot
        UpdateUi(currentSlot); //Updates the UI
        objGrab.gameObject.transform.SetParent(null); //Removes its child status 
        if (destroy) {Destroy(objGrab.gameObject);}
        if (currentSlot.count > 0) { //If there is still items left in the slot put one into the players hand 
            NextItemFromSlot(currentSlot);
        } else {
            CurrentSlotEmpty(currentSlot);
        }
        return true;
    }
    
    public bool RemoveItemFromAnywhere(ObjectGrab objGrab,  InventorySlot slot, bool destroy = false) { //Passing in the item you are removing and if you are destroying it or not (e.g eating it)
        if (slot.count <= 0 || slot.items.Count <= 0) {return false;} //If there is no items in the slot return false
        if (slot.count == 10) {slot.full = false;}
        slot.items.Remove(objGrab);
        slot.count = slot.items.Count;  //Remove the item from the list of items in the slot
        UpdateUi(slot); //Updates the UI
        objGrab.gameObject.transform.SetParent(null); //Removes its child status 
        if (destroy) {Destroy(objGrab.gameObject);}
        if (slot == currentSlot && slot.count > 0) { //If there is still items left in the slot (and the slot is selected) put one into the players hand 
            NextItemFromSlot(slot);
        } else if (slot == currentSlot) {
            CurrentSlotEmpty(currentSlot);
        }
        return true;
    }

    //Switching slots

    public void SwitchSlot(int slotNum) {
        //if (slotNum == currentSlotIndex) { return; }
        currentSlot.UI.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.5f); //Make the previous slot white again
        currentSlotIndex = slotNum;
        currentSlot = slots[currentSlotIndex]; //Updates the index and the current slot to the new one
        if (objectInHand != null) { //If there is an object in the hand make it inactive and clear the object in hand var
            objectInHand.gameObject.SetActive(false);
            objectInHand = null;
        }
        NextItemFromSlot(currentSlot); //Get the next item from the new slot or null if the slot is empty
       
        currentSlot.UI.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.5f); //Make the new slot dark
    }

    //Crafting 

    public bool HasItems(string itemType, int itemCount) {
        int localCount = 0;
        foreach (InventorySlot slot in slots) {
            if (slot.type == itemType) {
                localCount += slot.count;
            }
        }
        if (localCount >= itemCount) {
            return true;
        } else {
            return false;
        }
    }
    public int HowManyCraft(List<IngredientsType> ingredients) {
        int maxCraftable = int.MaxValue;
        foreach (IngredientsType ingredient in ingredients) {
            int totalAvailable = 0;
            foreach (InventorySlot slot in slots) {
                if (slot.type == ingredient.name) {
                    totalAvailable += slot.count;
                }
            }
            int possibleWithThisIngredient = totalAvailable / ingredient.count;
            if (possibleWithThisIngredient < maxCraftable) {
                maxCraftable = possibleWithThisIngredient;
            }
        }
        return maxCraftable == int.MaxValue ? 0 : maxCraftable;
    }
    public void TakeItemsForCrafting(List<IngredientsType> ingredients) {
        foreach (IngredientsType ingredient in ingredients) {
            int countRemaining = ingredient.count;
            foreach (InventorySlot slot in slots) {
                if (slot.type == ingredient.name) {
                    for (int i = slot.items.Count - 1; i >= 0 && countRemaining > 0; i--) {
                        ObjectGrab item = slot.items[i];
                        RemoveItemFromAnywhere(item, slot, true);
                    }
                }
                // Stop if we've collected enough
                if (countRemaining <= 0) {
                    break;
                }
            } 
        }
    }
    
    //Inventory UI

    
    public void UpdateUi(InventorySlot slot) {
        TextMeshProUGUI text = GetTextObject(slot.UI); //Gets the Ui text
        Image itemImage = GetChildsImage(slot.UI); //Gets the object ui image
        text.SetText(slot.count.ToString()); //Updates the count on the ui
        if (slot.count >= 1) { //If there is items in the slot
            itemImage.gameObject.SetActive(true); //Show the text and the image 
            text.gameObject.SetActive(true);
            itemImage.GetComponent<Image>().sprite = FindTextureByName(slot.type); //Set the image texture
        } else { //Hide the text and the image if there is no items in the slot
            itemImage.gameObject.SetActive(false); 
            text.gameObject.SetActive(false);
        }
    }

    public Sprite FindTextureByName(string textureName) { //Finds the corresponding texture for the ui matching the object type
        foreach (Sprite obj in objectImages) {
            // Remove any instance-specific suffixes like " (Instance)"
            string cleanName = obj.name.Replace(" (Instance)", "");
            if (cleanName == textureName) {
                return obj;
            }
        }
        Debug.LogWarning($"Material named '{textureName}' not found.");
        return null;
    }

    public Image GetChildsImage(GameObject parent) { //Gets the image that the item is displayed on from the main ui slot container
        Transform child = parent.transform.GetChild(0);
        Image imageLink = child.GetComponent<Image>();
        return imageLink;
    }
    
    public TextMeshProUGUI GetTextObject(GameObject parent) { //Get the childs text for the slots ui
        TextMeshProUGUI textLink = parent.GetComponentInChildren<TextMeshProUGUI>(true);
        return textLink;
    }
}
   
    // // Shake trees when clicked and not holding anything
    // private void TryShakeTree(RaycastHit hit) {
    //     if (objectInHand == null) {
    //         hit.transform.GetComponent<TreeScript>()?.Shake();
    //     }
    // }

    // // Try to start a fire (two twigs) or smash rocks (two rocks)
    // private void TryFireOrSmash(RaycastHit hit) {
    //     if (objectInHand == null) return;

    //     ObjectGrab objectOnFloor = hit.transform.GetComponent<ObjectGrab>();
    //     if (objectOnFloor == null || objectOnFloor.holdingPoint != null) return;

    //     string inHandType = objectInHand.GetObjectType();
    //     string onFloorType = objectOnFloor.GetObjectType();

    //     if (inHandType == "Twig" && onFloorType == "Twig") {
    //         if (ValidPlaceSpot() && RemoveItem(objectInHand)) {
    //             startFire.TryStartFire(objectOnFloor, objectInHand);
    //             toolTips.objectInHand = objectInHand;
    //         }
    //     } else if (inHandType == "Rock" && onFloorType == "Rock") {
    //         if (ValidPlaceSpot() && RemoveItem(objectInHand)) {
    //             rockSmash.TrySmashRocks(objectOnFloor, objectInHand);
    //             toolTips.objectInHand = objectInHand;
    //         }
    //     }
    // }

    // // Harvest deer when right-clicking a dead one
    // private void TryHarvestDeer(RaycastHit hit) {
    //     DeerMovement deer = hit.transform.GetComponent<DeerMovement>();
    //     if (deer != null && deer.dead) {
    //         spawnManager.SpawnMeat(hit.transform.position);
    //         Destroy(deer.gameObject);
    //     }
    // }
    // private void TryRaycastInteraction(int mouseButton, float distance, LayerMask layerMask, System.Action<RaycastHit> onHit) {
    //     if (Input.GetMouseButtonDown(mouseButton)) {
    //         if (Physics.Raycast(cameraPosition.position, cameraPosition.forward, out RaycastHit hit, distance, layerMask)) {
    //             onHit?.Invoke(hit);
    //         }
    //     }
    // }
    // void Update() {
    //     if (startGame.gameStarted) {
    //         // CheckForSlotSwitch();
    //         // CheckForItemPickup();
    //         // CheckForItemPlaced();
    //         // CheckForTreeClicked();
    //         // CheckForFoodEaten();
    //         // CheckForFireAttempt();
    //         // CheckForRockSmash();
    //         // CheckForSpearThrowStart();
    //         // CheckForDeerHarvest();
    //         // TryRaycastInteraction(0, maxDistance, treesLayer, TryShakeTree);
    //         // TryRaycastInteraction(1, maxDistance, objectsLayer, TryFireOrSmash);
    //         // TryRaycastInteraction(1, maxDistance, animalsLayer, TryHarvestDeer);
    //     }   
    // }
// IEnumerator LerpSlot(InventorySlot slot, Vector2 start, Vector2 end) {
    //     RectTransform currentRt = slot.UI.GetComponent<RectTransform>();
    //     float t = 0f;
    //     float duration = 0.5f;  // Duration in seconds

    //     while (t < 1f) {
    //         t += Time.deltaTime / duration;
    //         currentRt.anchoredPosition = Vector2.Lerp(start, end, t);
    //         yield return null;  // <-- This keeps the coroutine running frame-by-frame
    //     }

    //     // Ensure it snaps exactly to the final position at the end
    //     currentRt.anchoredPosition = end;
    // }


    // private void MoveSlots(int slotNum) {
    //     StartCoroutine(LerpSlot(currentSlot, selectedPos, currentSlot.defaultPos));
    //     RectTransform rt = slots[slotNum].UI.GetComponent<RectTransform>();
    //     if (slots[slotNum].defaultPos == Vector2.zero) {
    //         slots[slotNum].defaultPos = rt.anchoredPosition;
    //     }
    //     StartCoroutine(LerpSlot(slots[slotNum], rt.anchoredPosition, selectedPos));
    // }
