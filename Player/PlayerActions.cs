using UnityEngine;
public class PlayerActions : MonoBehaviour {
    [Header("Layer Masks")]
    [SerializeField] private LayerMask objectsLayer;
    [SerializeField] private LayerMask treesLayer;
    [SerializeField] private LayerMask terrainLayer;
    [SerializeField] private LayerMask animalsLayer;
    [Header("For Raycasting out from the camera")]
    [SerializeField] private Transform cameraPosition; 
    private readonly float maxDistance = 10f;
    public ObjectGrab objectInHand; //Current object in the players hand
    [SerializeField] private Transform holdPoint; //Point the object in the players hand is kept at
    [SerializeField] private Inventory inventory; //The players inventory
    private bool dropTimeBlock; //Blocking items from being dropped
    [SerializeField] private StartGame startGame; //Checking if the game is started or not
    [Header("Other files that perfom the actions the player can do")]
    [SerializeField] private SpawnManager spawnManager; //For spawning items
    [SerializeField] private StartFire startFire; //Starting fires
    [SerializeField] private RockSmash rockSmash; //Smashing rocks
    [SerializeField] private SpearThrow spearThrow; //Throwing spears
    [SerializeField] private PlayerStats playerStats; //The players stats
    [SerializeField] private SaveOverTime saveOverTime; //For showing the object in hand

    //Checks if the place the player is looking at is a valid spot on the terrain to place an object
    private Vector3 ValidPlaceSpot() {
        if (Physics.Raycast(cameraPosition.position, cameraPosition.forward, out RaycastHit raycastHit, maxDistance, terrainLayer)) { //If the raycast hits the terrain
            Vector3 point = raycastHit.point;
            return point;
        }
        else {
            return Vector3.zero;
        }
    }
    //Checks if the the spot is valid and the item can be removed from the inventory
    private bool ValidSpotAndRemovedFromInv() {
        Vector3 point = ValidPlaceSpot();
        ObjectGrab prevObject = objectInHand;
        if (point != Vector3.zero) {
            bool removed = inventory.RemoveItem(objectInHand);
            if (!removed) {return false;}
            prevObject.Drop(point); //Release the object from the hand
            return true;
        } else {
            return false;
        }
    }
    //Checks if 2 objects are the same type
    private bool SameType(string obj1, string obj2, string type) {
        if (obj1 == type && obj2 == type) {
            return true;
        } else {
            return false;
        }
    }
    //Helper function for when the player needs to look at something and click 
    private void TryRaycastInteractionOnClick(int mouseButton, float distance, LayerMask layerMask, System.Action<RaycastHit> onHit) {
        if (Input.GetMouseButtonDown(mouseButton)) {
            if (Physics.Raycast(cameraPosition.position, cameraPosition.forward, out RaycastHit hit, distance, layerMask)) {
                onHit?.Invoke(hit);
            }
        }
    }
    //Helper function for when the player needs to look at something and press a key
    private void TryRaycastInteractionOnKey(KeyCode keyCode, float distance, LayerMask layerMask, System.Action<RaycastHit> onHit) {
        if (Input.GetKeyDown(keyCode)) {
            if (Physics.Raycast(cameraPosition.position, cameraPosition.forward, out RaycastHit hit, distance, layerMask)) {

                onHit?.Invoke(hit);
            }
        }
    }
    //Harvest deer when right-clicking a dead one
    private void TryHarvestDeer(RaycastHit hit) {
        DeerMovement deer = hit.transform.GetComponent<DeerMovement>();
        if (deer != null && deer.dead) {
            spawnManager.SpawnMeat(hit.transform.position);
            Destroy(deer.gameObject);
        }
    }
    //Start a fire with 2 twigs or smash 2 rocks together to get stones
    private void TryFireOrSmash(RaycastHit hit) {
        if (objectInHand == null) return;
        ObjectGrab prevObjInHand = objectInHand;
        ObjectGrab objectOnFloor = hit.transform.GetComponent<ObjectGrab>();
        if (objectOnFloor == null || objectOnFloor.holdingPoint != null) return;

        string inHandType = objectInHand.oType;
        string onFloorType = objectOnFloor.oType;

        if (SameType(inHandType, onFloorType, "Twig")) {
            if (ValidSpotAndRemovedFromInv()) {
                startFire.TryStartFire(objectOnFloor, prevObjInHand);
                //toolTips.objectInHand = objectInHand;
            }
        } else if (SameType(inHandType, onFloorType, "Rock")) {
            if (ValidSpotAndRemovedFromInv()) {
                rockSmash.TrySmashRocks(objectOnFloor, prevObjInHand); //copy
                //toolTips.objectInHand = objectInHand;
            }
        }
    }
    private void TryChopLog(RaycastHit hit) {
        if (objectInHand == null) return;
        ObjectGrab objectOnFloor = hit.transform.GetComponent<ObjectGrab>();
        if (objectOnFloor == null || objectOnFloor.holdingPoint != null) return;
        if (objectOnFloor.oType == "Log" && objectInHand.oType == "Axe") {
            if (objectOnFloor.pickupAble && objectOnFloor.choppable) {
                objectInHand.Chop();
                //saveOverTime.savingItems.Remove(objectOnFloor); //Remove the object in hand from the saving list
                StartCoroutine(InvokeChopLog(objectOnFloor.gameObject, 0.8f));
            }
        }
    }

    private System.Collections.IEnumerator InvokeChopLog(GameObject logObject, float delay) {
        yield return new WaitForSeconds(delay);
        spawnManager.ChopLog(logObject);
    }
    //Shakes a tree
    private void TryShakeOrChopTree(RaycastHit hit) {
        if (objectInHand == null) {
            hit.transform.GetComponent<TreeScript>()?.Shake();
        }
        else if (objectInHand.oType == "Axe") {
            TreeScript tree = hit.transform.GetComponent<TreeScript>();
            objectInHand.Chop();
            StartCoroutine(InvokeChopTree(tree, 0.8f));
        }
    }
    private System.Collections.IEnumerator InvokeChopTree(TreeScript tree, float delay) {
        yield return new WaitForSeconds(delay);
        tree.Chop();
    }
    //Switches the current inventory slot
    private void TrySwitchSlot() {
        for (int i = 1; i <= 9; i++) {
            KeyCode topRowKey = KeyCode.Alpha0 + i;
            if (Input.GetKeyDown(topRowKey)) {
                inventory.SwitchSlot(i - 1); //-1 because indexing starts from 0
            }
        }
    }
    //Picking up items
    private void TryPickUp(RaycastHit hit) {
        ObjectGrab objectPickedUp = hit.transform.GetComponent<ObjectGrab>(); //Gets the object grab script
        if (objectPickedUp.holdingPoint == null && objectPickedUp.pickupAble == true) { //If you can pick up the object
            if (inventory.AddItem(objectPickedUp)) { //If the object can be added into the inventory
                objectPickedUp.Grab(holdPoint); //Tell the object to go to the holdingpoint
            }
        }
            
    }
    //Dropping items
    private void TryPlaceItem() {
        if (Input.GetKeyDown(KeyCode.Q) && objectInHand != null && !dropTimeBlock) {
            if (ValidSpotAndRemovedFromInv()) { //If there is a spot to place it and the item can be removed from the inventory
                dropTimeBlock = true;
                Invoke(nameof(ResetDropTimer), 1); //1 second delay on dropping items
            }
        }
    }
    private void ResetDropTimer() {
        dropTimeBlock = false;
    }
    //Eating food
    private void TryEatFood() {
        if (Input.GetMouseButtonDown(0) && objectInHand != null) {
            if (objectInHand.isEdible && playerStats.hungerValue < 100) { //If the object is edible and the player does not have full hunger
                int foodVal = objectInHand.foodValue; //How much hunger the food gives
                if (inventory.RemoveItem(objectInHand, true)) { //If the item can be removed from the inventory (also destroyed)
                    playerStats.UpdateHunger(foodVal); //Increases the players hunger
                }
            } 
        }
    }
    //Throwing a spear
    private void TryThrowSpear() {
        if (Input.GetMouseButtonDown(0) && objectInHand != null) {
            if (objectInHand.oType == "Spear") {
                spearThrow.StartCharge();
            }
        }
    }
    public void ThrowSpear(float force) {
        ObjectGrab spear = objectInHand;
        if (inventory.RemoveItem(objectInHand)) {
            spear.Throw(force, cameraPosition);
        }
    }
    void Update() {
        if (!startGame.gameStarted) return;
        //Interacting with stuff
        TryRaycastInteractionOnClick(0, maxDistance, treesLayer, TryShakeOrChopTree); //Shake tree
        TryRaycastInteractionOnClick(1, maxDistance, objectsLayer, TryFireOrSmash); //Create fire or Smash rocks
        TryRaycastInteractionOnClick(0, maxDistance, objectsLayer, TryChopLog); //Create fire or Smash rocks
        TryRaycastInteractionOnClick(1, maxDistance, animalsLayer, TryHarvestDeer); //Harvest meat from dead deer

        //Inventory
        TryRaycastInteractionOnKey(KeyCode.E, maxDistance, objectsLayer, TryPickUp); //Pick up object
        TryPlaceItem(); //Place object in hand
        TrySwitchSlot(); //Switch current inventory slot

        //Using item in hand
        TryEatFood(); //Eat food 
        TryThrowSpear(); //Throw spear

    }

}
//  private void CheckForItemPickup() { 
//         if (Input.GetKeyDown(KeyCode.E)) {
//             if (Physics.Raycast(cameraPosition.position, cameraPosition.forward, out RaycastHit raycastHit, maxDistance, objectsLayer)) { //If the raycast hits an object
//                 ObjectGrab objectPickedUp = raycastHit.transform.GetComponent<ObjectGrab>(); //Gets the object grab script
//                 if (objectPickedUp.holdingPoint == null && objectPickedUp.pickupAble == true) { //If you can pick up the object
//                     if (AddItem(objectPickedUp)) { //If the object can be added into the inventory
//                         objectPickedUp.Grab(holdPoint); //Tell the object to go to the holdingpoint
//                     }
//                 }
//             }
//         }
//     }
//     private bool ValidPlaceSpot() {
//         if (Physics.Raycast(cameraPosition.position, cameraPosition.forward, out RaycastHit raycastHit, maxDistance, terrainLayer)) { //If the raycast hits the terrain
//             Vector3 point = raycastHit.point;
//             objectInHand.Drop(point); //Release the object from the hand
//             return true;
//         } else {
//             return false;
//         }
//     }
//     private void CheckForItemPlaced() {
//         if (Input.GetKeyDown(KeyCode.Q) && objectInHand != null && !dropTimeBlock) {
//             if (ValidPlaceSpot() && RemoveItem(objectInHand)) { //If there is a spot to place it and the item can be removed from the inventory
//                 dropTimeBlock = true;
//                 Invoke(nameof(ResetDropTimer), 1); //1 second delay on dropping items
//                 toolTips.objectInHand = objectInHand;
//             }
//         }
//     }
//     private void CheckForTreeClicked() {
//         if (Input.GetMouseButtonDown(0) && objectInHand == null) { //If there is not food in hand (eating priority)
//             if (Physics.Raycast(cameraPosition.position, cameraPosition.forward, out RaycastHit raycastHit, maxDistance, treesLayer)) { //If the raycast hits a tree
//                 TreeScript tree = raycastHit.transform.GetComponent<TreeScript>();
//                 tree.Shake(); //Shakes the tree
//             } 
//         }
//     }
//     private void CheckForSpearThrowStart() {
//         if (Input.GetMouseButtonDown(0) && objectInHand != null) {
//             if (objectInHand.GetObjectType() == "Spear") {
//                 spearThrow.StartCharge();
//             }
//         }
//     }
//     public void ThrowSpear(float force) {
//         ObjectGrab spear = objectInHand;
//         if (RemoveItem(objectInHand)) {
//             Debug.Log("Throw Spear Called");
//             spear.Throw(force, cameraPosition);
//         }
//     }
//     private void CheckForFoodEaten() {
//         if (Input.GetMouseButtonDown(0) && objectInHand != null) {
//             if (objectInHand.GetEdible() && playerStats.GetHunger() < 100) { //If the object is edible and the player does not have full hunger
//                 int foodVal = objectInHand.GetFoodValue(); //How much hunger the food gives
//                 if (RemoveItem(objectInHand, true)) { //If the item can be removed from the inventory (also destroyed)
//                     playerStats.UpdateHunger(foodVal); //Increases the players hunger
//                     toolTips.objectInHand = objectInHand;
//                 }
//             } 
//         }
//     }
//     private void CheckForFireAttempt() {
//         if (Input.GetMouseButtonDown(1)) {
//             if (Physics.Raycast(cameraPosition.position, cameraPosition.forward, out RaycastHit raycastHit, maxDistance, objectsLayer)) {
//                 ObjectGrab objectOnFloor = raycastHit.transform.GetComponent<ObjectGrab>();
//                 bool bothTwigs = objectOnFloor.GetObjectType() == "Twig" && objectInHand.GetObjectType() == "Twig";
//                 if (objectOnFloor.holdingPoint == null && objectInHand != null && bothTwigs) {
//                     if (ValidPlaceSpot() && RemoveItem(objectInHand)) {
//                         startFire.TryStartFire(objectOnFloor, objectInHand);
//                         toolTips.objectInHand = objectInHand;
//                     }
//                 }
//             }
//         }
//     }
//     private void CheckForRockSmash() {
//         if (Input.GetMouseButtonDown(1)) {
//             if (Physics.Raycast(cameraPosition.position, cameraPosition.forward, out RaycastHit raycastHit, maxDistance, objectsLayer)) {
//                 ObjectGrab objectOnFloor = raycastHit.transform.GetComponent<ObjectGrab>();
//                 bool bothRocks = objectOnFloor.GetObjectType() == "Rock" && objectInHand.GetObjectType() == "Rock";
//                 if (objectOnFloor.holdingPoint == null && objectInHand != null && bothRocks) {
//                     if (ValidPlaceSpot() && RemoveItem(objectInHand)) {
//                         rockSmash.TrySmashRocks(objectOnFloor, objectInHand);
//                         toolTips.objectInHand = objectInHand;
//                     }
//                 }
//             }
//         }
//     }
//     private void CheckForDeerHarvest() {
//         if (Input.GetMouseButtonDown(1)) {
//             if (Physics.Raycast(cameraPosition.position, cameraPosition.forward, out RaycastHit raycastHit, maxDistance, animalsLayer)) {
//                 DeerMovement deerMovement = raycastHit.transform.GetComponent<DeerMovement>();
//                 if (deerMovement.dead) {
//                     spawnManager.SpawnMeat(raycastHit.transform.position);
//                     Destroy(deerMovement.gameObject);
//                 }
//             }
//         }
//     }
//private void CheckForItemPickup() { 
    //     if (Input.GetKeyDown(KeyCode.E)) {
    //         if (Physics.Raycast(cameraPosition.position, cameraPosition.forward, out RaycastHit raycastHit, maxDistance, objectsLayer)) { //If the raycast hits an object
    //             ObjectGrab objectPickedUp = raycastHit.transform.GetComponent<ObjectGrab>(); //Gets the object grab script
    //             if (objectPickedUp.holdingPoint == null && objectPickedUp.pickupAble == true) { //If you can pick up the object
    //                 if (inventory.AddItem(objectPickedUp)) { //If the object can be added into the inventory
    //                     objectPickedUp.Grab(holdPoint); //Tell the object to go to the holdingpoint
    //                 }
    //             }
    //         }
    //     }
    // }