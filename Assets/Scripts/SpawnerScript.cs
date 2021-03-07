using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerScript : MonoBehaviour
{

    public float frequencyInSeconds = 10f;
    public GameObject spawnee;
    public GameObject spawneeRare;
    private float halfHeight = 1.5f;

    void Start()
    {
        TurnOn();
    }

    public void TurnOn(){
        InvokeRepeating("Spawn", 2.0f, frequencyInSeconds);
    }
    public void TurnOff(){
        CancelInvoke();
    }

    void Spawn()
    {
        // Debug.Log("Spawn()");
        GameObject prefab = Random.Range(0,10) == 0 ? spawneeRare : spawnee;
        GameObject spawnedGO = Instantiate(prefab, transform.position+new Vector3(0,halfHeight,0), transform.rotation);
        // Debug.Log("spawnedGO: " + spawnedGO);
    }

}
