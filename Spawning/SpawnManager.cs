using UnityEngine;

public class SpawnManager : MonoBehaviour {
    public Terrain terrain;
    [Header("Environment Objects")]
    public GameObject appleTree;
    public GameObject pearTree;
    public GameObject pineTree;
    public GameObject normalTree;
    [Header("Food items")]
    public GameObject apple;
    public GameObject pear;
    public GameObject meat;
    [Header("Enviroment items")]
    public GameObject rock;
    public GameObject twig;
    public GameObject stone;
    public GameObject log;
    [Header("Tools")]
    public GameObject axe;
    public GameObject spear;
    [Header("Animals")]
    public GameObject deer;
    [Header("Particles")]
    public ParticleSystem fire;
    [Header("Terrain Info")]
    private Vector3 terrainPosition;
    private Vector3 terrainSize;
    // Use a Dictionary to map item names to prefabs for easier lookup
    private readonly System.Collections.Generic.Dictionary<string, GameObject> itemPrefabs = new System.Collections.Generic.Dictionary<string, GameObject>();
    private readonly System.Collections.Generic.Dictionary<string, GameObject> treePrefabs = new System.Collections.Generic.Dictionary<string, GameObject>();

    void Awake() {
        terrainPosition = terrain.transform.position;
        terrainSize = terrain.terrainData.size;
        itemPrefabs["twig"] = twig;
        itemPrefabs["stone"] = stone;
        itemPrefabs["meat"] = meat;
        itemPrefabs["rock"] = rock;
        itemPrefabs["log"] = log;
        itemPrefabs["apple"] = apple;
        itemPrefabs["pear"] = pear;
        itemPrefabs["deer"] = deer;
        itemPrefabs["spear"] = spear;
        itemPrefabs["axe"] = axe;
        treePrefabs["AppleTree"] = appleTree;
        treePrefabs["PearTree"] = pearTree;
        treePrefabs["PineTree"] = pineTree;
    }

    public Vector3 GetRandomPosition(float offsetY) {
        float randomX = Random.Range(0, terrainSize.x);
        float randomZ = Random.Range(0, terrainSize.z);
        float worldX = terrainPosition.x + randomX;
        float worldZ = terrainPosition.z + randomZ;

        float terrainY = terrain.SampleHeight(new Vector3(worldX, 0, worldZ));
        float worldY = terrainY + terrainPosition.y + offsetY;
        return new Vector3(worldX, worldY, worldZ);
    }
    public static Quaternion GetRandomEulerRotation() {
        float x = Random.Range(0f, 360f);
        float y = Random.Range(0f, 360f);
        float z = Random.Range(0f, 360f);
        return Quaternion.Euler(x, y, z);
    }

    private Vector3 Offset() {
        float x = Random.Range(-2, 2);
        float y = 5;
        float z = Random.Range(-2, 2);
        return new Vector3(x, y, z);
    }
    public void DropLog(string type, Vector3 position) {
        Vector3 pos = position;
        Instantiate(log, pos, log.transform.rotation);
    }
    public void DropFruit(string type, Vector3 position) {
        Vector3 pos = position + Offset();
        GameObject prefab = apple;
        if (type == "AppleTree") {
            prefab = apple;
        }
        else if (type == "PearTree") {
            prefab = pear;
        }
        Instantiate(prefab, pos, prefab.transform.rotation);
    }
    public void DropTwig(string type, Vector3 position) {
        Vector3 pos = position + Offset();
        Instantiate(twig, pos, twig.transform.rotation);
    }
    public void SpawnMeat(Vector3 position) {
        position.y += 0.25f;
        for (int i = 0; i < Random.Range(1, 4); i++) {
            Instantiate(meat, position, GetRandomEulerRotation());
        }
    }
    public ParticleSystem SpawnFire(Vector3 position) {
        ParticleSystem ps = Instantiate(fire, position, fire.transform.rotation);
        ps.Play();
        return ps;
    }
    public void BreakUpRock(Vector3 position, int count, GameObject rock1, GameObject rock2) {
        for (int i = 0; i < count; i++) {
            Vector3 rocksPos = position;
            rocksPos.x += Random.Range(-0.1f, 0.1f);
            rocksPos.z += Random.Range(-0.1f, 0.1f);
            Instantiate(stone, rocksPos, Quaternion.identity);
        }
        Destroy(rock1);
        Destroy(rock2);
    }
    public void ChopLog(GameObject log) {
        Vector3 pos = log.transform.position;
        Vector3 scale = log.transform.localScale;
        Vector3 newScale = new Vector3(scale.x, scale.y * 0.5f, scale.z);
        for (int i = 0; i <= 1; i++) {
            GameObject newLog = Instantiate(log, pos, GetRandomEulerRotation());
            newLog.transform.localScale = newScale;
            if (newScale.y < 0.25f) {
                newLog.transform.GetComponent<ObjectGrab>().choppable = false;
            }
        }
        Destroy(log);
    }


    public GameObject SpawnItem(string itemName, Vector3 position, Quaternion rotation) {
        if (itemName == null) { return null; }
        if (itemPrefabs.TryGetValue(itemName, out GameObject prefab) && prefab != null) {
            position.y += 0.25f; 
            GameObject spawned = Instantiate(prefab, position, rotation);
            Debug.Log($"Spawned {itemName}(s) at {position}");
            return spawned;
        }
        else {
            Debug.LogWarning($"Unknown item name: {itemName}");
            return null;
        }
    }
    public GameObject SpawnTree(string treeType, Vector3 position, Quaternion rotation) {
        if (treeType == null) { return null; }
        if (treePrefabs.TryGetValue(treeType, out GameObject prefab) && prefab != null) {
            position.y += 0.25f; 
            GameObject spawned = Instantiate(prefab, position, rotation);
            Debug.Log($"Spawned {treeType}(s) at {position}");
            return spawned;
        }
        Debug.LogWarning($"Unknown tree type: {treeType}");
        return null;
        
    }
}

// void SpawnDeer() {
    //     for (int i = 0; i < 50; i++) {
    //         Instantiate(deer, GetRandomPosition(5f), deer.transform.rotation);

    //     }
    // }

        // void SpawnTrees() {
    //     foreach (GameObject tree in trees) {
    //         for (int i = 0; i < 2; i++) {
    //             GameObject treeNew = Instantiate(tree, GetRandomPosition(0f), tree.transform.rotation);
    //             GameObject trunkObstacle = new("TrunkObstacle"); 
    //             trunkObstacle.transform.parent = treeNew.transform;
    //             trunkObstacle.AddComponent<NavMeshObstacle>().carving = true;
    //             trunkObstacle.transform.localPosition = new Vector3(0, 1f, 0);  
    //             trunkObstacle.transform.localScale = new Vector3(1f, 2f, 1f);
    //             trunkObstacle.transform.localRotation = Quaternion.identity;
    //             treeNew.GetComponent<TreeScript>().trunkObstacle = trunkObstacle;
    //             //tree.transform.SetParent(treeHolder, false);
    //         }
    //     }
    // }
    // void SpawnRocks() {
    //     foreach (GameObject rock in rocks) {
    //         for (int i = 0; i < 5; i++) {
    //             Instantiate(rock, GetRandomPosition(0.5f), rock.transform.rotation);
    //         }
    //     }
    // }
