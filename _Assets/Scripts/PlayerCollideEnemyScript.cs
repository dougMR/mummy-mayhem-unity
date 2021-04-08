using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollideEnemyScript : MonoBehaviour
{
    // Start is called before the first frame upsdate
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    // When Player Trigger Collides w/ Mummy, Mummy chase Player
    private void OnTriggerEnter(Collider other)
    {
        // Debug.Log("other.Name: " + other.name);
        if (other.name.Contains("Mummy"))
        {
            // Start following me
            other.GetComponent<EnemyAIScript>().ChasePlayer();
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.name.Contains("Mummy"))
        {
            // Stop following me
            other.GetComponent<EnemyAIScript>().StopChasePlayer();
        }
    }
}
