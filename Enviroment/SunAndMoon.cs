using TMPro;
using UnityEngine;
//using UnityEngine.UI;
public class DayNightCycle : MonoBehaviour
{
    [Range (0f, 360f)] [SerializeField] private float timeOfDay = 0f; //0/360 - midday, 90 - sunset, 180 - midnight, 270 - sunrise
    [SerializeField] private float hours; //how many hours have passed that day
    [SerializeField] private int currentHour; //Current hour of the day
    [SerializeField] private int currentMin; //Current minute of the hour
    [SerializeField] private TextMeshProUGUI timeText; //Reference to the time text 
    [Range (-1,1)] [SerializeField] float sunStrength; //Strength of the sun (-1 at midnight and 1 at midday)
    [Range (-1,1)] [SerializeField] float moonStrength; //Strength of the moon (-1 at midday and 1 at midnight)
    public Light sunLight; //Reference to the sun light
    public Light moonLight; //Reference to the moon light (inactive as both do not work together)
    [SerializeField] private Camera mainCamera; //The main camera
    [SerializeField] private float renderDistance; //Current render distance
    [SerializeField] private PlayerStats playerStats;
    [Range (0,100)] [SerializeField] private int temp; //Current ambient temperature


    void Awake() {
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Skybox;
        RenderSettings.ambientIntensity = 1f;

        DynamicGI.UpdateEnvironment();
    }
    void SetTime() {
        hours = timeOfDay/ 360f * 24f;
        //tofday = timeOfDay/360;
        currentHour = Mathf.FloorToInt(hours); 
        currentMin = Mathf.FloorToInt((hours - currentHour)*60f);
        timeText.SetText(currentHour.ToString() + "." + currentMin.ToString());
    }
    void SetFog() {
        if (currentHour < 5 || currentHour > 19) {
            renderDistance = 100f;
        } else {
           renderDistance = 1000f;
        }
        mainCamera.farClipPlane = renderDistance;
    }
    void AdvanceTime() {
        timeOfDay += Time.deltaTime * 20f; //Speed at which time passes
        if (timeOfDay > 360f) {
            timeOfDay = 0f;
        }
        sunStrength = -Mathf.Cos(Mathf.PI/180 * timeOfDay);
        moonStrength = Mathf.Cos(Mathf.PI/180 * timeOfDay);
        transform.rotation = Quaternion.Euler(timeOfDay, 0f, 0f);
    }
    void SetTemp() {
        if (sunStrength > 0 ) {
            temp = Mathf.FloorToInt(Mathf.Lerp(0f, 100f, sunStrength));
            playerStats.SetTemp(temp);
        } else {
            playerStats.SetTemp(0);
        }
    }

    void Update() {
        if (currentHour < 12) { //Stopping at midday for testing
        AdvanceTime();
        }
        SetTime();
        SetFog();
        SetTemp();
    }
}

