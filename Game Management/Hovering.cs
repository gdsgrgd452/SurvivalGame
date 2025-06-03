using UnityEngine;
using UnityEngine.EventSystems;

public class HoverDetector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public bool isHovering = false;
    [SerializeField] Crafting crafting;
    public int index;

    public void OnPointerEnter(PointerEventData eventData) {
        isHovering = true;
        crafting.ShowCraftingInfo(gameObject, index);
        //Debug.Log("Hovering over " + gameObject.name);
    }

    public void OnPointerExit(PointerEventData eventData) {
        isHovering = false;
        crafting.HideCraftingInfo();
        //Debug.Log("Stopped hovering over " + gameObject.name);
    }
}