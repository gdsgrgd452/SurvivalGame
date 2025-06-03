using System.IO;
using System.Collections.Generic;
using UnityEngine;

public static class SaveSystem {
    
    private static string folderPath = Application.persistentDataPath + "/saves";

    // Ensure the folder exists
    private static void EnsureFolder() {
        if (!Directory.Exists(folderPath)) {
            Directory.CreateDirectory(folderPath);
        }
    }

    public static void Save(SaveData data) {
        EnsureFolder();
        string json = JsonUtility.ToJson(data, true);
        string path = Path.Combine(folderPath, data.saveName + ".json");
        File.WriteAllText(path, json);
    }

    public static SaveData Load(string saveName) {
        string path = Path.Combine(folderPath, saveName + ".json");
        if (File.Exists(path)) {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<SaveData>(json);
        }
        Debug.LogWarning($"Save file '{saveName}' not found!");
        return null;
    }

    public static List<string> GetAllSaveNames() {
        EnsureFolder();
        string[] files = Directory.GetFiles(folderPath, "*.json");
        List<string> saveNames = new List<string>();
        foreach (string file in files) {
            saveNames.Add(Path.GetFileNameWithoutExtension(file));
        }
        return saveNames;
    }

    public static void DeleteSave(string saveName) {
        string path = Path.Combine(folderPath, saveName + ".json");
        if (File.Exists(path)) {
            File.Delete(path);
        }
    }
}
