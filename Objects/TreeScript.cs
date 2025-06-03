using System.Collections;
using UnityEngine;
public class TreeScript : MonoBehaviour {
    [SerializeField] private SpawnManager spawnManager;
    public string type;
    private int fruitCount = 5;
    public bool destroyed = true; // Track if the tree has been destroyed
    public GameObject trunkObstacle;

    void Awake() {
        spawnManager = FindAnyObjectByType<SpawnManager>();
    }

    public void Shake() {
        int fruitChance = Random.Range(1, 3);
        if (fruitChance < 10 && fruitCount > 0)
        {
            fruitCount--;
            Debug.Log("Shook");
            spawnManager.DropFruit(type, transform.position);
            spawnManager.DropTwig(type, transform.position);
        }
    }
    public void Chop() {
        Debug.Log("Chopped Tree");
        // spawnManager.DropFruit(type, transform.position);
        // spawnManager.DropTwig(type, transform.position);
        //spawnManager.DropWood(type, transform.position);

        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.mass = 20f; // Make it heavier like a tree

        // Apply a torque to make the tree fall over in a random direction
        Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
        rb.AddTorque(Vector3.Cross(Vector3.up, randomDirection) * 500f);

        // Optionally, add a small forward force to help it tip
        rb.AddForce(randomDirection * 2f, ForceMode.Impulse);

        // Start coroutine to check when the tree has finished falling
        StartCoroutine(DropTwigsWhenFallen(rb));
    }

    private IEnumerator DropTwigsWhenFallen(Rigidbody rb) {
        yield return new WaitForSeconds(0.5f); // Wait a bit before checking velocity
        // Wait until the tree's velocity is low (fallen and settled)
        while (rb.linearVelocity.magnitude > 0.1f || rb.angularVelocity.magnitude > 0.1f)
        {
            yield return null;
        }

        // Drop a random amount of twigs (e.g., 1 to 4)
        int logCount = Random.Range(2, 3);
        for (int i = 0; i < logCount; i++) {
            spawnManager.DropLog(type, transform.position);
        }
        destroyed = true; // Mark the tree as destroyed
        gameObject.SetActive(false); // Optionally deactivate the tree object
    }
}
