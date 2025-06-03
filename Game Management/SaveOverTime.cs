using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class SaveOverTime : MonoBehaviour {
    [SerializeField] private GameObject player;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private MoveForward playerController;
    [SerializeField] private Inventory inventory;
    [SerializeField] private float saveInterval = 5f; // Time in seconds between saves
    private Coroutine saveCoroutine;
    public List<ObjectGrab> savingItems;

    public void StartSaving() {
        saveCoroutine = StartCoroutine(SavePlayerData());
    }
    private SavedInvSlot[] ConvertToSavedInvSlots() {
        SavedInvSlot[] savedSlots = new SavedInvSlot[9];
        for (int i = 0; i < 9; i++) {
            savedSlots[i] = new SavedInvSlot {
                slotIndex = i,
                type = inventory.slots[i].type,
                count = inventory.slots[i].count
            };
        }
        //Debug.Log($"Saved Inventory Slot: {savedSlots[0].slotIndex}, Type: {savedSlots[0].type}, Count: {savedSlots[0].count}");
        return savedSlots;
    }
    private List<SavedItem> ConvertToSavedItems() {
        List<SavedItem> savedItems = new List<SavedItem>();
        foreach (ObjectGrab item in savingItems) {
            savedItems.Add(new SavedItem {
                position = item.transform.position,
                rotation = item.transform.rotation,
                type = item.oType,
                });
            //Debug.Log($"Saved Item: {item.oType}, Position: {item.transform.position}, Rotation: {item.transform.rotation}");
        }
        return savedItems;
    }
    private List<SavedTree> ConvertToSavedTrees() {
        List<SavedTree> savedTrees = new List<SavedTree>();
        TreeScript[] trees = FindObjectsByType<TreeScript>(FindObjectsSortMode.None);
        foreach (TreeScript tree in trees) {
            savedTrees.Add(new SavedTree {
                position = tree.transform.position,
                rotation = tree.transform.rotation,
                type = tree.type.ToLower(),
                destroyed = tree.destroyed
            });
            //Debug.Log($"Saved Tree: {tree.type}, Position: {tree.transform.position}, Rotation: {tree.transform.rotation}, Destroyed: {tree.destroyed}");
        }
        return savedTrees;
    }
    private IEnumerator SavePlayerData() {
        while (true) {
            yield return new WaitForSeconds(saveInterval);
            SaveData data = new SaveData {
                saveName = "save1",
                playerPosition = player.transform.position,
                playerRotation = player.transform.rotation,
                health = playerStats.healthValue,
                stamina = playerController.stamina,
                hunger = playerStats.hungerValue,
                temp = playerStats.tempValue,
                currentInvSlot = inventory.currentSlotIndex,
                inventorySlots = ConvertToSavedInvSlots(),
                items = ConvertToSavedItems(),
                trees = ConvertToSavedTrees()
            };
            SaveSystem.Save(data);
            //Debug.Log($"Position: {data.playerPosition}, Rotation: {data.playerRotation}, Health: {data.health},  Hunger: {data.hunger}");
        }
    }
}
