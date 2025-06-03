using UnityEngine;
using UnityEngine.AI;
using System.Collections;
public class DeerMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    [SerializeField] private SpawnManager spawnManager;
    private bool hasArrived;
    public string type = "Deer";
    [SerializeField] private Animator animator;
    [SerializeField] private bool eating;
    [SerializeField] private bool idle;
    [SerializeField] private bool run;
    [SerializeField] private string currentAnimation;
    [SerializeField] private float animationTime;
    [SerializeField] private float lVel;
    [SerializeField] private float hitTorque;
    public bool dead;
    private Quaternion deadRotation;
    private Vector3 deadPosition;
    private Rigidbody deerRb;
    void Awake() {
        spawnManager = FindAnyObjectByType<SpawnManager>();
        deerRb = GetComponent<Rigidbody>();
        Invoke(nameof(OnStart), 2);
    }
    void OnStart() {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas)) {
            agent.Warp(hit.position); // Safer than setting transform.position directly
        }
        if (agent.isOnNavMesh && !hasArrived) {
            MoveTo(spawnManager.GetRandomPosition(0f));
        }
    }

    public void MoveTo(Vector3 targetPosition) {
        if (agent != null && agent.enabled && agent.isOnNavMesh) {
            hasArrived = false;
            animator.SetFloat("Speed_f", 1.0f);
            agent.SetDestination(targetPosition);
            
        } else {
            Debug.Log($"Agent Enabled: {agent.enabled}, OnNavMesh: {agent.isOnNavMesh}");
        }
    }
    void Update() {
        if (!hasArrived && agent != null && agent.enabled && agent.isOnNavMesh && !agent.pathPending) {
            lVel = agent.velocity.magnitude;
            animator.SetFloat("Speed_f", lVel/10);
            if (agent.remainingDistance <= agent.stoppingDistance && (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)) {
                hasArrived = true;
                animator.SetFloat("Speed_f", 0f);
                StartCoroutine(Eat());
            }
        }
        
    }
    public void Hit(float fallAngle) {
        hasArrived = true; // Prevent further AI movement logic
        animator.SetFloat("Speed_f", 0f); // Stop movement animation
        animator.enabled = false;
        deerRb.linearVelocity = Vector3.zero;
        agent.enabled = false; // Disable the NavMeshAgent component
        Fall(fallAngle);
    }
    private void Fall(float zRotationTarget) {
        Transform tr = transform;
        float rotationSpeed = 12f; // degrees per second
        Vector3 currentEuler = tr.localEulerAngles;
        float currentZ = currentEuler.z;
        Quaternion finalRotation = Quaternion.Euler(tr.eulerAngles.x, tr.eulerAngles.y, zRotationTarget);
        Physics.IgnoreLayerCollision(6, 10, true);
        Physics.IgnoreLayerCollision(6, 11, true);
        dead = true;

        while (Mathf.Abs(currentZ - zRotationTarget) > 0.1f) {
            float step = rotationSpeed * Time.deltaTime; // Calculate the step for this frame
            currentZ = Mathf.MoveTowards(currentZ, zRotationTarget, step); // Move currentZ towards targetZ by step
            tr.localEulerAngles = new Vector3(currentEuler.x, currentEuler.y, currentZ); // Apply the new rotation while keeping X and Y unchanged
            deadPosition = transform.position;
            deadPosition.y += 0.5f;
            deadRotation = transform.rotation;
            deerRb.linearVelocity = Vector3.zero;
            deerRb.angularVelocity = Vector3.zero; 
        }
        tr.rotation = finalRotation;
        deerRb.linearVelocity = Vector3.zero;
        deerRb.angularVelocity = Vector3.zero; 
        Physics.IgnoreLayerCollision(6, 10, false);
        Physics.IgnoreLayerCollision(6, 11, false);
        //GetComponent<Collider>().isTrigger = true;

        
    }


    IEnumerator Eat() {
        float duration = Random.Range(10f, 60f); //How long the deer stays still for
        float elapsed = 0f;
        
        while (elapsed < duration) { 
            // Start eating
            animator.SetBool("Eat_b", true);
            animator.SetBool("Idle_b", false);
            float eatTime = Random.Range(2f, 5f);
            yield return new WaitForSeconds(eatTime);
            elapsed += eatTime;

            // Switch to idle
            animator.SetBool("Eat_b", false);
            animator.SetBool("Idle_b", true);
            float idleTime = Random.Range(2f, 4f);
            yield return new WaitForSeconds(idleTime);
            elapsed += idleTime;
        }
        // Done eating/idle, clear animation states
        animator.SetBool("Idle_b", false);
        animator.SetBool("Eat_b", false);
        
        // Move somewhere else
        Vector3 newPos = spawnManager.GetRandomPosition(0f);
        MoveTo(newPos);
    }
    void LateUpdate() {
        if (dead && (transform.rotation != deadRotation || transform.position != deadPosition)) {
            transform.SetPositionAndRotation(deadPosition, deadRotation);
            transform.GetChild(0).rotation = deadRotation;
            deerRb.angularVelocity = Vector3.zero;
        }
    }
}



