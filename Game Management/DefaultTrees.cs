// using System.Collections.Generic;
// using UnityEngine;

// public class DefaultTrees : MonoBehaviour {
//         // Example method to save default trees
//     public void SaveDefaultTrees()
//     {
//         List<SavedTree> defaultTrees = new();
//         foreach (var tree in Object.FindObjectsByType<TreeScript>(FindObjectsSortMode.None))
//         {
//             defaultTrees.Add(new SavedTree
//             {
//                 position = tree.transform.position,
//                 rotation = tree.transform.rotation,
//                 type = tree.type.ToLower(),
//                 destroyed = false // All trees are alive in the default setup
//             });
//         }
//         // Save to a file (implement SaveDefaultTreesToFile as needed)
//         SaveSystem.SaveDefaultTreesToFile(defaultTrees);
//     }
// }
