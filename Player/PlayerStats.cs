using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerStats : MonoBehaviour {
    //UI elements and value for health
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI healthText;
    public int healthValue = 100;
    //UI elements for damage
    [SerializeField] private Image damageImage;
    private Coroutine damageCoroutine;
    private Color damageColor = new Color(1f, 0f, 0f, 1f);
    //UI elements and value for hunger
    [SerializeField] private Slider hungerSlider;
    [SerializeField] private TextMeshProUGUI hungerText;
    public int hungerValue = 100;
    private bool hungerEnabled = true;
    //UI elements and value for temperature
    public int tempValue = 0;
    [SerializeField] private Slider tempSlider;
    [SerializeField] private Image tempSliderFill;
    [SerializeField] private TextMeshProUGUI tempText;
    //UI elements for freezing effects
    [SerializeField] private Image freezeImage;
    private Coroutine freezingCoroutine;
    private Color freezeColor = new Color(0.2f, 0.8f, 0.8f, 1f);
    //Objects near player (for heating from fire)
    [SerializeField] private ObjectsNearPlayer objsNearPlayer;
    //Warning if the player is in danger
    [SerializeField] private TextMeshProUGUI warningText;

    public void GameStart(int health, int hunger, int temp) {
        SetHealth(health);
        SetHunger(hunger);
        SetTemp(temp);
        StartCoroutine(HungerOverTime());
    }
    void Update() {
        // Calculate normalized value (0 to 1)
        float t = tempSlider.value / 100;

        // Lerp the color
        tempSliderFill.color = Color.Lerp(Color.blue, Color.red, t);
    }
    public void UpdateHealth(int value) {
        healthValue += value;
        healthValue = Mathf.Min(healthValue, 100);
        healthValue = Mathf.Max(healthValue, 0);
        healthSlider.value = healthValue;
        healthText.SetText(healthValue.ToString() + "/100");
    }
    public void SetHealth(int health) {
        healthValue = health;
        UpdateHealth(0);
    }
    private void WarnPlayer(bool onOff, string text, Color color) {
        warningText.SetText(text);
        warningText.color = new Color(1f, 0f, 0f, 1f);
        warningText.gameObject.SetActive(onOff);
    }
    public void UpdateTemp(int value) {
        tempValue += value;
        tempValue = Mathf.Min(tempValue, 100);
        tempValue = Mathf.Max(tempValue, 0);
        tempSlider.value = tempValue;
        tempText.SetText(tempValue.ToString() + "*C");
        if (tempValue > 10 && freezingCoroutine != null) {
            StopCoroutine(Freezing());
            freezingCoroutine = null;
            WarnPlayer(false, "", Color.white);
        } else if (tempValue <= 10 && freezingCoroutine == null) {
            freezingCoroutine = StartCoroutine(Freezing());
            WarnPlayer(true, "You are freezing!, Get a fire going", freezeColor);
        }
    }
    public void SetHunger(int hunger) {
        hungerValue = hunger;
        UpdateHunger(0);
    }
    public void UpdateHunger(int value) {
        hungerValue += value;
        hungerValue = Mathf.Min(hungerValue, 100);
        hungerValue = Mathf.Max(hungerValue, 0);
        hungerSlider.value = hungerValue;
        hungerText.SetText(hungerValue.ToString() + "/100");
        if (hungerValue > 19 && damageCoroutine != null) {
            StopCoroutine(Starving());
            damageCoroutine = null;
        } else if (hungerValue < 20 && damageCoroutine == null) {
            damageCoroutine = StartCoroutine(Starving());
        }
    }
    public void SetTemp(int temp) {
        tempValue = (int)(temp + objsNearPlayer.totalHeat*15);
        UpdateTemp(0);
    }
    IEnumerator HungerOverTime() {
        while (hungerEnabled) {
            UpdateHunger(-1);
            yield return new WaitForSeconds(Random.Range(1,10));
            if (hungerValue < 20 && damageCoroutine == null) {
                damageCoroutine = StartCoroutine(Starving());   
            }
        }
    }
    private void DamageEffect() {
        float duration = 1f;
        float elapsed = 0f;
        while (elapsed < duration) {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(0.1f, 0.5f, elapsed / duration);
                damageImage.color = new Color(1f, 0f, 0f, alpha);
            }
            // Fade out
            elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(0.5f, 0.1f, elapsed / duration);
                damageImage.color = new Color(1f, 0f, 0f, alpha);
            }
            damageImage.color = new Color(1f, 0f, 0f, 0f);
        }
    
    IEnumerator Starving() {
        while (true) {
            // Fade in
            UpdateHealth(-1);
            DamageEffect();
            yield return new WaitForSeconds(0.2f);
            if (hungerValue > 19) {break;}
        }
    }
    IEnumerator Freezing() {
        float duration = 5f;
        float elapsed = 0f;
        while (true) {
            // Fade in
            UpdateHealth(-1);
            //Debug.Log("Playing");
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(0.1f, 0.2f, elapsed / duration);
                freezeImage.color = new Color(0.2f, 0.8f, 0.8f, alpha);
                yield return null;
            }
            // Fade out
            elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(0.2f, 0.1f, elapsed / duration);
                freezeImage.color = new Color(0.2f, 0.8f, 0.8f, alpha);
                yield return null;
            }
            if (tempValue > 10) {
                freezeImage.color = new Color(0.2f, 0.8f, 0.8f, 0f);
                break;
            }
        }
    }
}
