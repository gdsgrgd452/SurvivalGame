using System.Collections;
using UnityEngine;

public class Burn : MonoBehaviour
{
    [SerializeField] private LayerMask objectsLayer; //Layer mask for checking for nearby objects
    [SerializeField] private ObjectsNearPlayer objsNearPlayer;
    private GameObject fireParticles;
    [SerializeField] private ObjectGrab objectGrab;
    public bool flammable; //If it can be burned
    public float burnTime; //How long it takes to burn
    public bool isOnFire; //If the object is on fire currently

    void Awake() {
        objsNearPlayer = FindAnyObjectByType<ObjectsNearPlayer>();
        objectGrab = transform.GetComponent<ObjectGrab>();
    }
    
    public void SetOnFire(GameObject fire) {
        fireParticles = fire;
        fireParticles.transform.position = transform.position;
        objectGrab.pickupAble = false;
        isOnFire = true;
        StartCoroutine(BurnOverTime());
    }
    private void SpreadFire() {
        Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, 1f, objectsLayer); //Gets all objects within a 1 unit sphere
        foreach (Collider col in nearbyObjects) {
            Burn otherStick = col.gameObject.GetComponent<Burn>(); 
            ObjectGrab otherStickGrab = col.gameObject.GetComponent<ObjectGrab>();
            bool found = otherStick != null && otherStickGrab != null; //Checks the other object has the objectGrab and Burn scripts
            if (found && !otherStick.isOnFire && otherStickGrab.holdingPoint == null) { //If the object is not on fire and not in the players hand
                objsNearPlayer.Caught(otherStick);
                //Debug.Log("Caught");
            }
        }
    }
    IEnumerator BurnOverTime() {
        while (burnTime > 0) {
            burnTime--;
            yield return new WaitForSeconds(1f);
            SpreadFire();
            float distanceToPlayer = (objsNearPlayer.playerPos - transform.position).magnitude;
            objsNearPlayer.ItemCheck(gameObject, distanceToPlayer);
            if (burnTime <= 0) {
                //Debug.Log("Fire gone");
                objsNearPlayer.StickGone(gameObject);
                Destroy(fireParticles);
                Destroy(gameObject);
            }
        }
    }
    void FixedUpdate() {
        if (fireParticles != null) {
            fireParticles.transform.position = transform.position;
        }
    }
}
