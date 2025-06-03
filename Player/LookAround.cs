using UnityEngine;

public class LookAround : MonoBehaviour
{
    public float sensitivity = 200;
    public GameObject player;
    public float xRotation = 0f;
    private Rigidbody playerRb;
    public bool blockRotation;
    public bool freeCursor;
    [SerializeField] private StartGame startGame;
    void Start() {
        playerRb = player.GetComponent<Rigidbody>();
        
    }
    void Update() {
        if (Input.GetKeyDown(KeyCode.P) && !freeCursor) {
            blockRotation = !blockRotation;
        }
    }
    public void FreeCursorForAction(bool free) {
        freeCursor = free;
        if (freeCursor) {
            Cursor.lockState = CursorLockMode.None;
            blockRotation = true;
        } else {
            Cursor.lockState = CursorLockMode.Locked;
            blockRotation = false;
        }
    }
    void LateUpdate() {
        if (!blockRotation && startGame.gameStarted) {
            //Gets the mouse inputs
            float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
            //Sets the rotation to the mouse inputs
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            player.transform.Rotate(Vector3.up * mouseX);
        }   
    }
}
