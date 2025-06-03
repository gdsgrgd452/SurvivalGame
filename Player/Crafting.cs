using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Crafting : MonoBehaviour {
    [SerializeField] private GameObject craftingMenu;
    private bool menuOpen;
    [System.Serializable] public class IngredientsType { //E.g 2x wood or 5x apples
        public string name;
        public int count;
    }
    [System.Serializable] public class CraftingItem { //E.g spear, ingredients, spear prefab, true/false
        public string name;
        public List<IngredientsType> ingredients;
        public GameObject prefab;
        public bool available;
        public int amountAvailable;
        public GameObject uiSlot;
    }
    [SerializeField] private List<CraftingItem> itemsAvailable;
    [SerializeField] private StartGame startGame;
    [SerializeField] private Inventory inventory;
    [SerializeField] private LookAround lookAround;
    private Coroutine checkWhichAreCraftable;
    [SerializeField] private Transform contentPanel;
    [SerializeField] private ScrollRect scrollRect;
    //[SerializeField] private int maxVisibleItems = 7;
    [SerializeField] private float scrollSensitivity = 0.1f;
    [SerializeField] private List<GameObject> uiSlots;
    [SerializeField] private GameObject infoText;
    [SerializeField] private CraftingItem hoveringOver;
    private bool hovering;
    void Start() {
        scrollRect.scrollSensitivity = scrollSensitivity;
    }
    void OpenMenu() {
        craftingMenu.SetActive(menuOpen);
        contentPanel.gameObject.SetActive(menuOpen);
        lookAround.FreeCursorForAction(menuOpen);
        if (menuOpen && checkWhichAreCraftable == null) {
            checkWhichAreCraftable = StartCoroutine(RepeatCheck());
            HideUnusedUi();
        } else if (!menuOpen && checkWhichAreCraftable != null) {
            StopCoroutine(checkWhichAreCraftable);
            checkWhichAreCraftable = null;
        }
    }
    private void EditBox(CraftingItem craftingItem, bool available, string name, int count) {
        Transform tr = craftingMenu.transform.GetChild(itemsAvailable.IndexOf(craftingItem));
        craftingItem.available = available;
        craftingItem.amountAvailable = count;
        craftingItem.name = name;
        tr.gameObject.SetActive(available);

    }
    public void ShowCraftingInfo(GameObject box, int index) {
        infoText.SetActive(true);
        infoText.transform.position = box.transform.position;
        hoveringOver = itemsAvailable[index];
        infoText.transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText(hoveringOver.name);
        infoText.transform.GetChild(1).GetComponent<TextMeshProUGUI>().SetText(hoveringOver.amountAvailable.ToString() + " X");
        hovering = true;
    }
    public void HideCraftingInfo() {
        infoText.SetActive(false);
        hoveringOver = null;
        hovering = false;
    }
    private void HideUnusedUi() {
        uiSlots.Clear();
        foreach (Transform child in craftingMenu.transform) {
            uiSlots.Add(child.gameObject);
        }
        foreach (GameObject uiSlot in uiSlots) {
            uiSlot.SetActive(false);
            foreach (CraftingItem craftingItem in itemsAvailable) {
                if (craftingItem.uiSlot == uiSlot) {
                    uiSlot.GetComponent<Image>().sprite = inventory.FindTextureByName(craftingItem.name);
                    uiSlot.GetComponent<HoverDetector>().index = itemsAvailable.IndexOf(craftingItem);
                    uiSlot.SetActive(true);
                    //Debug.Log("Ssfgew");
                }
            }
        }
    }
    IEnumerator RepeatCheck() {
        while (true) {
            foreach (CraftingItem craftingItem in itemsAvailable) {
                bool available = true;
                foreach (IngredientsType ingredientsType in craftingItem.ingredients) {
                    if (!inventory.HasItems(ingredientsType.name, ingredientsType.count)) {
                        craftingItem.available = false;
                    }
                }
                int howMany = inventory.HowManyCraft(craftingItem.ingredients);
                if (craftingItem.available != available || craftingItem.amountAvailable != howMany) {
                    EditBox(craftingItem, available, craftingItem.name, howMany);
                }
            }
            yield return new WaitForSeconds(20f);
       }
        
    }
    void Craft() {
        bool available = true;
        foreach (IngredientsType ingredientsType in hoveringOver.ingredients) {
            if (!inventory.HasItems(ingredientsType.name, ingredientsType.count)) {
                available = false;
            }
        }
        if (available) {
            Instantiate(hoveringOver.prefab, inventory.gameObject.transform.position, Quaternion.identity);
            inventory.TakeItemsForCrafting(hoveringOver.ingredients);
            menuOpen = false;
            OpenMenu();
            HideCraftingInfo();
        }
    }
    void Update() {
        if (startGame.gameStarted && Input.GetKeyDown(KeyCode.C)) {
            menuOpen = !menuOpen;
            OpenMenu();
        } 
        if (hovering && Input.GetMouseButtonDown(0)) {
            if (hoveringOver != null) {
                Craft();
            } 
        }
    }
}
