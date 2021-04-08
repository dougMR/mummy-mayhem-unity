using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketPickupScript : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
        {

            PlayerManager.Instance.AddWeaponByName("Rocket Launcher");
            
            Destroy(gameObject);
        }
    }
}
