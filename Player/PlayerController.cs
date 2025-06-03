using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
public class MoveForward : MonoBehaviour
{
    [SerializeField] private float speed = 3;
    public Rigidbody playerRb;
    [SerializeField] PlayerStats playerStats;
    private Coroutine sprintCoroutine;
    private bool gameStarted;
    [SerializeField] private Slider staminaSlider;
    [SerializeField] private bool regenStamina;
    private Coroutine regenCoroutine;
    private bool isSprinting;
    private float regenDelay = 3f;
    [SerializeField] private float playerHeight = 1.9f;
    public float stamina = 100f;
    private float maxStamina = 100f;
    private Coroutine savePositionCoroutine;
    private bool canControl = false;

    public void GameStart(Vector3 playerPosition, Quaternion playerRotation, float staminaValue) {
        gameStarted = true;
        transform.position = playerPosition;
        transform.rotation = playerRotation;
        stamina = staminaValue;
        canControl = false;
        StartCoroutine(EnableControlAfterDelay(1f));
    }

    private IEnumerator EnableControlAfterDelay(float delay) {
        yield return new WaitForSeconds(delay);
        canControl = true;
    }

    void FixedUpdate() {
        if (gameStarted && canControl) {
            Move();
            Jump();
            Sprint();
            if (regenStamina) {
                stamina += Time.deltaTime;
            }
            if (staminaSlider.value != stamina) {
                staminaSlider.value = Mathf.MoveTowards(staminaSlider.value, stamina, 10f * Time.deltaTime);
            }
        }
    }

    // Jumping
    void Jump() {
        if (Input.GetKeyUp(KeyCode.Space) && IsGrounded() && stamina >= 15) {
            Debug.Log("Jump");
            playerRb.AddForce(Vector3.up * 1000, ForceMode.Impulse);
            stamina -= 10f;
            stamina = Mathf.Clamp(stamina, 0, maxStamina);
            RestartStaminaRegenDelay();
        } 
    }
    // Check if the player is grounded
    bool IsGrounded() {
        return Physics.Raycast(transform.position, Vector3.down, playerHeight); // Adjust distance to match player height
    }

    // Player movement
    void Move() {
        float moveInputForward = Input.GetAxis("Vertical");
        float moveInputSideways = Input.GetAxis("Horizontal");
        Vector3 move = new Vector3(moveInputSideways, 0f, moveInputForward);
        if (move.magnitude > 1f) {
            move.Normalize(); // Prevent diagonal speed boost
        }
        transform.Translate(move * speed * Time.deltaTime);
    }
    // Sprinting
    void Sprint() {
        bool sprinting = Input.GetKey(KeyCode.LeftShift) && playerStats.hungerValue > 15 && staminaSlider.value > 0;

        if (sprinting) {
            stamina -= 3f * Time.deltaTime;
            stamina = Mathf.Clamp(stamina, 0, maxStamina);
            speed = 12;
            isSprinting = true;
        } else {
            speed = 3;
            isSprinting = false;
            regenCoroutine ??= StartCoroutine(RegenerateStamina());
        }
    }

    //Stamina regeneration
    private void RestartStaminaRegenDelay() {
        if (regenCoroutine != null) {
            StopCoroutine(regenCoroutine);
            regenCoroutine = null;
        }
        regenCoroutine = StartCoroutine(RegenerateStamina());
    }
    IEnumerator RegenerateStamina() {
        yield return new WaitForSeconds(regenDelay);

        while (stamina < 100 && !isSprinting && IsGrounded()) {
            stamina += 1f * Time.deltaTime;
            stamina = Mathf.Clamp(stamina, 0, maxStamina);
            yield return null;
        }
        regenCoroutine = null;
    }

    // Hunger goes down faster when running
    IEnumerator HungryRunning() {
        while (true) {
            yield return new WaitForSeconds(2f);
            int hungerChance = Random.Range(1, 25);
            if (hungerChance < 2) {
                playerStats.UpdateHunger(-1);
            }
        }
    }
    

}
