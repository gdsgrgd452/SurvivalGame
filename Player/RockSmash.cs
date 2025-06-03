using UnityEngine;

public class RockSmash : MonoBehaviour {
    [SerializeField] private ObjectGrab rockOnFloor;
    [SerializeField] private ObjectGrab rockNew;
    [SerializeField] private LookAround lookAround;
    [SerializeField] private SpawnManager spawnManager;

    public void TrySmashRocks(ObjectGrab obj1, ObjectGrab obj2) {
        rockOnFloor = obj1;
        rockNew = obj2;
        Debug.Log(rockNew, rockOnFloor);
        rockNew.gameObject.transform.position = rockOnFloor.gameObject.transform.position + Vector3.up * 0.5f;
        Smash();
    }
    void Smash() {
        Vector3 newRockPosition = rockNew.transform.position;
        spawnManager.BreakUpRock(newRockPosition, Random.Range(4,10), rockNew.gameObject, rockOnFloor.gameObject);
    }
}