using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectGrab : MonoBehaviour
{   
    private Rigidbody objectRb;

    [Header("For keeping it in the players hand")]
    public Transform holdingPoint;
    private float LerpSpeed = 30f;
    private Quaternion holdOffsetSpear = Quaternion.Euler(77, 333, 80); //Rotation offset to align the item with the players hand
    private Quaternion holdOffsetAxe = Quaternion.Euler(322, 323, 72);
    Quaternion zeroRotation = Quaternion.Euler(322, 323, 0);
    Quaternion midRotation = Quaternion.Euler(322, 323, 72);
    private Quaternion thisOffset;

    [Header("Information about the object")]
    public string oType; //Objects type e.g Twig
    public bool isEdible; //If the object is edible
    public bool cookable; //If the object can be cooked
    public int foodValue; //How much hunger it gives when eaten
    public bool pickupAble = true; //If you can pick it up
    public bool choppable = true; //If the object can be chopped
    [SerializeField] private bool thrown; //If the object has been thrown
    [SerializeField] private float lVel; //Linear velocity
    public bool saving; //If the object is being saved
    
    private Collider playerCollider; //Collider of the player
    private Collider spearCollider; //Collider of the object only for spears
    [SerializeField] private SpearHit spearHit; //For checking for hits (spears only)
    [SerializeField] private SaveOverTime saveOverTime; //For saving the object over time

    void Awake() {
        objectRb = GetComponent<Rigidbody>();
        if (oType != "Axe") {
            thisOffset = holdOffsetSpear;
        }
        else {
            thisOffset = holdOffsetAxe;
        }
        saveOverTime = FindAnyObjectByType<SaveOverTime>();
        saveOverTime.savingItems.Add(this);
    }

    public void Grab(Transform point) {
        holdingPoint = point;
        pickupAble = false;
        objectRb.useGravity = false;
        saving = false; 
        saveOverTime.savingItems.Remove(this);
        //objectRb.freezeRotation = true;
    }

    public void Drop(Vector3 position) {
        holdingPoint = null;
        pickupAble = true;
        objectRb.useGravity = true;
        transform.position = position + Vector3.up * 0.5f;
        saving = true;
        saveOverTime.savingItems.Add(this);
    }

    public void Throw(float force, Transform camera) {
        playerCollider = camera.transform.GetComponentInParent<Collider>();
        spearCollider = gameObject.GetComponent<Collider>();
        Physics.IgnoreCollision(playerCollider, spearCollider);
        transform.position = camera.transform.position;
        holdingPoint = null;
        spearHit.thrown = true;
        thrown = true;
        pickupAble = false;
        objectRb.useGravity = true;
        objectRb.AddForce(30f * force * camera.forward, ForceMode.Impulse);
        saving = true;
        saveOverTime.savingItems.Add(this);
        //Debug.Log($"Spear Thrown {1000f * force * camera.forward}");
    }

    public void Chop() {
        StartCoroutine(Swing());
    }

    IEnumerator Swing() {
        float duration = 0.2f;
        float t = 0f;

        // Lerp to zero rotation
        while (t < 1f) {
            t += Time.deltaTime / duration;
            thisOffset = Quaternion.Slerp(holdOffsetAxe, zeroRotation, t);
            yield return null;
        }

        t = 0f;
        // Lerp to 180,180,180
        while (t < 1f) {
            t += Time.deltaTime / duration;
            thisOffset = Quaternion.Slerp(zeroRotation, midRotation, t);
            yield return null;
        }

        t = 0f;
        // Lerp back to initial rotation (322,323,72)
        while (t < 1f) {
            t += Time.deltaTime / duration;
            thisOffset = Quaternion.Slerp(midRotation, holdOffsetAxe, t);
            yield return null;
        }
    }

    void OnCollisionEnter(Collision collision) {
        if (thrown) {
            if (collision.gameObject.CompareTag("Terrain")) {
                spearHit.thrown = false;
                thrown = false;
                pickupAble = true;
                Physics.IgnoreCollision(playerCollider, spearCollider, false);
            } 
        }
    }

    void StopSaving() {
        saving = false;
        saveOverTime.savingItems.Remove(this);
    }

    private void FixedUpdate() {
        if (holdingPoint != null) {
            Vector3 newPos = Vector3.Lerp(transform.position, holdingPoint.position, Time.deltaTime * LerpSpeed); //Keeps the object in the 
            objectRb.MovePosition(newPos);
            objectRb.transform.rotation = holdingPoint.transform.rotation * thisOffset;
        }
        else if (!thrown) {
            //objectRb.linearVelocity *= 0.95f;
            objectRb.angularVelocity *= 0.95f; //Slows down the rotation of the object
            lVel = objectRb.linearVelocity.magnitude;
            if (lVel < 0.15) {
                objectRb.linearVelocity = Vector3.zero;
                //Invoke(nameof(StopSaving), 3f); //Stops saving after 1 seconds
            }

        }
    }
}

