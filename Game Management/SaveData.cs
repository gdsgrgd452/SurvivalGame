using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class SaveData {
    public string saveName;
    public Vector3 playerPosition;
    public Quaternion playerRotation;
    public int health;
    public float stamina;
    public int hunger;
    public int temp;
    public int currentInvSlot;
    public SavedInvSlot[] inventorySlots;
    public List<SavedItem> items;
    public List<SavedTree> trees;
}
[System.Serializable]
public class SavedInvSlot {
    public int slotIndex;
    public string type;
    public int count;
}
[System.Serializable]
public class SavedItem {
    public Vector3 position;
    public Quaternion rotation;
    public string type;
}
[System.Serializable]
public class SavedTree {
    public Vector3 position;
    public Quaternion rotation;
    public string type;
    public bool destroyed; 
}

