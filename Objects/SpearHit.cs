using UnityEngine;

public class SpearHit : MonoBehaviour
{
    [SerializeField] private Collider tipCollider;
    [SerializeField] private Collider spearCollider;
    public Transform stuckPoint;
    public bool thrown;
    //[SerializeField] private Quaternion stuckRotationOffset = Quaternion.Euler(271, 119, 180);

    void OnCollisionEnter(Collision collision) {
        //Debug.Log("Collision");
        if (collision.gameObject.CompareTag("Deer") && stuckPoint == null && thrown) {
            DeerMovement deer = collision.gameObject.GetComponent<DeerMovement>();
            if (!deer.dead) {
                ContactPoint contact = collision.contacts[0]; //Gets the contact point
                Vector3 hitPoint = contact.point; //Moves the anchor to the point of contact  
                Vector3 hitDirection = hitPoint - deer.transform.position;
                Vector3 localHitDirection = deer.transform.InverseTransformDirection(hitDirection);
                Debug.Log("Hit");
                float fallAngle = (localHitDirection.x > 0) ? 90f : -90f;
                deer.Hit(fallAngle);
            }
        }
    }
}