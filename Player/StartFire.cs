using UnityEngine;
using UnityEngine.UI;

public class StartFire : MonoBehaviour {
    [SerializeField] private Burn stickOnFloor;
    [SerializeField] private Burn stickNew;
    [SerializeField] private Slider fireSlider;
    [SerializeField] private LookAround lookAround;
    [SerializeField] private SpawnManager spawnManager;

    public void TryStartFire(ObjectGrab obj1, ObjectGrab obj2) {
        stickOnFloor = obj1.transform.GetComponent<Burn>();
        stickNew = obj2.transform.GetComponent<Burn>();
        //Debug.Log($"At Start {stickOnFloor.gameObject.transform.position} {stickNew.gameObject.transform.position}");
        stickNew.gameObject.transform.position = stickOnFloor.gameObject.transform.position + Vector3.up * 0.5f;
        if (stickOnFloor.isOnFire || stickNew.isOnFire) {return;}
        lookAround.FreeCursorForAction(true);
        fireSlider.gameObject.SetActive(true);
        
    }
    void Update() {
        if (fireSlider.IsActive() == true && fireSlider.value == 100) {
            fireSlider.value = 0;
            fireSlider.gameObject.SetActive(false);
            lookAround.FreeCursorForAction(false);
            Vector3 floorStickPosition = stickOnFloor.transform.position;
            Vector3 newStickPosition = stickNew.transform.position;
            //Debug.Log($"At 100 {floorStickPosition} {newStickPosition}");
            ParticleSystem fire1 = spawnManager.SpawnFire(floorStickPosition);
            ParticleSystem fire2 = spawnManager.SpawnFire(newStickPosition);
            stickOnFloor.SetOnFire(fire1.gameObject);
            stickNew.SetOnFire(fire2.gameObject);
        }
    }
}
