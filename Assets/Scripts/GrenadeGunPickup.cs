using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeGunPickup : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
        {
            PlayerManager.Instance.AddWeaponByName("Grenade Thrower");
            Destroy(gameObject);
        }
    }
}

