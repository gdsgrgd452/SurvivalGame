using System.Collections.Generic;
using UnityEngine;

public class ObjectsNearPlayer : MonoBehaviour
{
    [SerializeField] private GameObject player;
    public Vector3 playerPos;
    public List<GameObject> burningSticksInRange;
    public float totalHeat;
    [SerializeField] private SpawnManager spawnManager;

    
    void Update() {
        if (burningSticksInRange.Count > 0) { //If the player is within range of burning items 
            CalculateHeat();
        }
    }
    private void CalculateHeat() {
        totalHeat = 0f;
        playerPos = player.transform.position;
        foreach (GameObject burningStick in burningSticksInRange) { //Calculates the heat for each stick
            float distanceToPlayer = (playerPos - burningStick.transform.position).magnitude;
            float heat = 2/distanceToPlayer; //Heat is more if the player is closer
            totalHeat += Mathf.Min(1f, heat);
        }        
    }
    public void StickGone(GameObject obj) { //When the stick is too far away remove or burns out remove it 
        burningSticksInRange.Remove(obj);
        CalculateHeat(); //Re calculate the heat
    }
    public void ItemCheck(GameObject obj, float distance) { 
        if (distance > 10f && burningSticksInRange.Contains(obj)) { //If the stick is not in range and in the list then remove it from the list
            burningSticksInRange.Remove(obj);
            CalculateHeat();
        } else if (distance <= 10f && !burningSticksInRange.Contains(obj)) { //If the stick is in range and not in the list then add it
            burningSticksInRange.Add(obj);
            CalculateHeat();
        }
    }
    public void Caught(Burn stick) { //Spawn a new fire if it catches
        ParticleSystem newFire = spawnManager.SpawnFire(stick.gameObject.transform.position);
        stick.SetOnFire(newFire.gameObject);
    }
}
