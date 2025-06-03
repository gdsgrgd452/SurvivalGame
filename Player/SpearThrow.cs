using UnityEngine;
using UnityEngine.UI;
public class SpearThrow : MonoBehaviour
{
    [SerializeField] private Slider throwCharge;
    [SerializeField] private bool active;
    [SerializeField] private PlayerActions playerActions;
    //[SerializeField] private ObjectGrab spear;

    public void StartCharge() {
        throwCharge.gameObject.SetActive(true);
        throwCharge.value = 0;
        active = true;
    }
    
    public void StopCharge() {
        playerActions.ThrowSpear(throwCharge.value);
        throwCharge.gameObject.SetActive(false);
        active = false;
    }   

    void Update() {
        if (active && Input.GetMouseButton(0)) {
            throwCharge.value += Time.deltaTime;
        } 
        if (active && Input.GetMouseButtonUp(0)) {
            StopCharge();
        }
    }
}
