using System.Collections.Generic;
using UnityEngine;

public class GiveItemsOnStart : MonoBehaviour
{
    //[SerializeField] private Inventory inventory;
    [SerializeField] private List<GameObject> items;
    //[SerializeField] private Transform holdPoint;
    [SerializeField] private GameObject player;
    public void GiveItems() {
        foreach (GameObject item in items) {
            Instantiate(item, player.transform.position, Quaternion.identity);
            //Debug.Log(item.transform.position, item);
            // ObjectGrab itemGrab = item.transform.GetComponent<ObjectGrab>();
            // if (inventory.AddItem(itemGrab)) {
            //     itemGrab.Grab(holdPoint);
            // }
        }
    }
}
