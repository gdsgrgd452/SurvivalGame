using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToolTips : MonoBehaviour {
    private Coroutine checkLooking;
    public ObjectGrab objectInHand;
    public Transform cameraPosition;
    private readonly float maxDistance = 10f;
    [SerializeField] private LayerMask objectsLayer;
    [SerializeField] private LayerMask treesLayer;
    [SerializeField] private LayerMask terrainLayer;
    [SerializeField] private LayerMask animalsLayer;
    [SerializeField] private LayerMask waterLayer;
    [SerializeField] private TextMeshProUGUI toolTip1;
    [SerializeField] private TextMeshProUGUI toolTip2;
    [SerializeField] private TextMeshProUGUI lookingAt;

    public void GameStart() {
        checkLooking = StartCoroutine(Look());
    }
    private void SetToolTipAndLookingAt(bool tt1, string ttext1, bool tt2, string ttext2, bool la, string latext) {
        toolTip1.gameObject.SetActive(tt1);
        toolTip1.SetText(ttext1);
        toolTip2.gameObject.SetActive(tt2);
        toolTip2.SetText(ttext2);
        lookingAt.gameObject.SetActive(la);
        lookingAt.SetText(latext);
    }
    private void TreeToolTip(RaycastHit raycastHit) {
        TreeScript tree = raycastHit.transform.GetComponent<TreeScript>();
        if (objectInHand != null && objectInHand.oType == "Axe") {
            SetToolTipAndLookingAt(true, "Click to chop", false, "", true, tree.type);
        } 
        else {
            SetToolTipAndLookingAt(true, "Click to shake", false, "", true, tree.type);
        }
        
    }
    private void ObjectToolTip(RaycastHit raycastHit1) {
        ObjectGrab objectLookedAt = raycastHit1.transform.GetComponent<ObjectGrab>();
        Burn objectFireInfo = raycastHit1.transform.GetComponent<Burn>();
        if (objectLookedAt.holdingPoint == null && objectLookedAt.pickupAble) { //Object is on the floor and pick up-able
            ObjectOnFloorToolTip(objectLookedAt);
        } else if (objectFireInfo != null && objectFireInfo.isOnFire) { //On fire
            SetToolTipAndLookingAt(true, "On fire!, Cant pick up", true, objectFireInfo.burnTime.ToString(), true, objectLookedAt.oType);
        } else if (objectLookedAt.holdingPoint != null) { //Object is in the players hand
            SetToolTipAndLookingAt(true, "Press Q to drop", false, "", true, objectLookedAt.oType);
        }
    }
    private void ObjectOnFloorToolTip(ObjectGrab objectLookedAt) {
        if (objectInHand != null && objectLookedAt.oType == "Twig" && objectInHand.oType == "Twig") { //Able to start fire
                SetToolTipAndLookingAt(true, "Press E to pickup", true, "Right click to start fire", true, objectLookedAt.oType);
        } else if (objectInHand != null && objectLookedAt.oType == "Rock" && objectInHand.oType == "Rock") { //Able to smash rocks
                SetToolTipAndLookingAt(true, "Press E to pickup", true, "Right Click to smash", true, objectLookedAt.oType);
        } else if (objectInHand != null && objectLookedAt.oType == "Log" && objectLookedAt.choppable && objectInHand.oType == "Axe") { //Able to chop log
                SetToolTipAndLookingAt(true, "Press E to pickup", true, "Click to chop", true, objectLookedAt.oType);
        } else { //Anything else on the floor
            SetToolTipAndLookingAt(true, "Press E to pickup", false, "", true, objectLookedAt.oType);
        } 
    }
    private void AnimalToolTip(RaycastHit raycastHit2) {
        DeerMovement deerMovement = raycastHit2.transform.GetComponent<DeerMovement>();
        if (deerMovement.dead) {
            SetToolTipAndLookingAt(true, "Rm button to harvest meat", false, "", true, deerMovement.type);
        } else {
            SetToolTipAndLookingAt(true, "Click to hit", false, "", true, deerMovement.type);
        }
    }
    // private void WaterToolTip(RaycastHit raycastHit3) {
    //     if (raycastHit3.transform.CompareTag("Water")) {
    //         return;
    //     }
    //     SetToolTipAndLookingAt(true, "Rm button to drink", false, "", true, "Water");
    // }
    // else if (Physics.Raycast(cameraPosition.position, cameraPosition.forward, out RaycastHit raycastHit3, maxDistance, terrainLayer)) {
    //             WaterToolTip(raycastHit3);
    //         } 
    IEnumerator Look() {
        while (true) {
            if (Physics.Raycast(cameraPosition.position, cameraPosition.forward, out RaycastHit raycastHit, maxDistance, treesLayer)) {
                TreeToolTip(raycastHit);
            } else if (Physics.Raycast(cameraPosition.position, cameraPosition.forward, out RaycastHit raycastHit1, maxDistance, objectsLayer)) {
                ObjectToolTip(raycastHit1);
            } else if (Physics.Raycast(cameraPosition.position, cameraPosition.forward, out RaycastHit raycastHit2, maxDistance, animalsLayer)) {
                AnimalToolTip(raycastHit2);
            } else if (objectInHand != null && objectInHand.isEdible == true) {
                SetToolTipAndLookingAt(true, "Click to eat", false, "", true, objectInHand.oType);
            } else {
                SetToolTipAndLookingAt(false, "", false, "", false, "");
            }
            yield return new WaitForSeconds(0.1f);
        }
        

    }
}
