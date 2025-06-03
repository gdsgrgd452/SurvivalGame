using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartGame : MonoBehaviour
{
    public bool gameStarted;
    [SerializeField] private Image preGameImage;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private ToolTips toolTips;
    [SerializeField] private MoveForward playerController;
    [SerializeField] private Inventory inventory;
    [SerializeField] private SpawnManager spawnManager;
    [SerializeField] private GiveItemsOnStart giveItemsOnStart;
    [SerializeField] private string currentSaveName = "save1";
    [SerializeField] private SaveOverTime saveOverTime;

    void NewSave() {
        // Create a new save file
        SaveData data = new SaveData {
            saveName = currentSaveName,
            playerPosition = new Vector3(500, 12, 500),
            playerRotation = Quaternion.identity,
            health = 100,
            stamina = 100,
            hunger = 100,
            temp = 0,
            currentInvSlot = 1,
            inventorySlots = new SavedInvSlot[9], // Initialize with an empty array
            items = new List<SavedItem>(),
            trees = new List<SavedTree>()
            
        };
        for (int i = 0; i < data.inventorySlots.Length; i++) {
                data.inventorySlots[i] = new SavedInvSlot {
                    slotIndex = i,
                    type = null,
                    count = 0,
                };
            }
        List<TreeScript> trees = new(Object.FindObjectsByType<TreeScript>(FindObjectsSortMode.None));
        foreach (var tree in trees) {
            data.trees.Add(new SavedTree {
                position = tree.transform.position,
                rotation = tree.transform.rotation,
                type = tree.type.ToLower(),
                destroyed = tree.destroyed
            });
        }
        SaveSystem.Save(data);
        SaveData loaded = SaveSystem.Load(currentSaveName);
        Debug.Log($"Loaded New save: Pos {loaded.playerPosition}, Hunger {loaded.hunger}");
        LoadPlayer(loaded.playerPosition, loaded.playerRotation, loaded.stamina);
        LoadPlayerStats(loaded.health, loaded.hunger, loaded.temp);
        LoadInventory(loaded.inventorySlots, loaded.currentInvSlot);
        StartTheGame();
    }

    void LoadSave() {
        // Load it
        SaveData loaded = SaveSystem.Load(currentSaveName);
        if (loaded == null) {
            Debug.Log("Failed to load save data.");
            NewSave();
            return;
        }
        Debug.Log($"Position: {loaded.playerPosition}, Rotation: {loaded.playerRotation}, Health: {loaded.health},  Hunger: {loaded.hunger}, Stamina: {loaded.stamina}"
        + $", Temp: {loaded.temp}, Current Inv Slot: {loaded.currentInvSlot}");
        LoadPlayer(loaded.playerPosition, loaded.playerRotation, loaded.stamina);
        LoadPlayerStats(loaded.health, loaded.hunger, loaded.temp);
        LoadInventory(loaded.inventorySlots, loaded.currentInvSlot);
        LoadItems(loaded.items);
        //LoadTrees(loaded.trees);
        StartTheGame();
    } 

    private void LoadInventory(SavedInvSlot[] savedSlots, int currentInvSlot = 1) {
        for (int i = 0; i < savedSlots.Length; i++) {
            for (int j = 0; j < savedSlots[i].count; j++) {
                GameObject item = spawnManager.SpawnItem(savedSlots[i].type?.ToLower(), inventory.transform.position, Quaternion.identity); // Spawns the items in the inventory
                inventory.AddItem(item.GetComponent<ObjectGrab>());
            }
        }
        inventory.LoadInventory(currentInvSlot); // Loads the inventory
    }

    private void LoadTrees(List<SavedTree> savedTrees) {
        foreach (SavedTree tree in savedTrees) {
            GameObject spawnedTree = spawnManager.SpawnTree(tree.type, tree.position, tree.rotation); // Spawns the trees in the world
            if (tree.destroyed) {
                spawnedTree.SetActive(false); // If the tree is destroyed, destroy it
            }
        }
    }

    private void LoadItems(List<SavedItem> savedItems) {
        Debug.Log($"Loading {savedItems.Count} trees.");
        foreach (SavedItem item in savedItems) {
            GameObject spawnedItem = spawnManager.SpawnItem(item.type.ToLower(), item.position, item.rotation); // Spawns the items in the world
            ObjectGrab objectGrab = spawnedItem.GetComponent<ObjectGrab>();
            if (objectGrab != null) {
                objectGrab.saving = true; // Set saving to true so it can be saved over time
            }
        }
    }

    private void LoadPlayer(Vector3 playerPosition, Quaternion playerRotation, float stamina = 100f) {
        playerController.GameStart(playerPosition, playerRotation, stamina); // Loads the players position, rotation and stamina
    }

    private void LoadPlayerStats(int health = 100,  int hunger = 100, int temp = 0) {
        playerStats.GameStart(health, hunger, temp); // Loads the player stats    
    }

    public void StartTheGame() {
        gameStarted = true; 
        preGameImage.gameObject.SetActive(false); // Hide the pre-game image
        toolTips.GameStart(); //Starts showing tooltips
        giveItemsOnStart.GiveItems(); // Gives the player items
        saveOverTime.StartSaving(); // Starts saving periodically
        Cursor.lockState = CursorLockMode.Locked;// Lock the cursor to the center of the screen and make it invisible
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space) && !gameStarted) {
            LoadSave();
        } else if (Input.GetKeyDown(KeyCode.S) && !gameStarted) {
            NewSave();
        }
    }

}

// // List all saves
        // List<string> saves = SaveSystem.GetAllSaveNames();
        // Debug.Log("Available saves:");
        // foreach (string s in saves) {
        //     Debug.Log($" - {s}");
        // }
