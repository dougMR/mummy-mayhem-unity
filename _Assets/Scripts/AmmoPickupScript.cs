using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickupScript : MonoBehaviour
{
    public int numAmmo = 10;
    public string ammoType = "Grenade Thrower";

    void OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.gameObject;
        if (other.name == "Player")
        {
            // Boost Player Ammo
            bool pickedUpAmmo = other.GetComponent<FireWeaponScript>().Reload(ammoType, numAmmo);
            if(pickedUpAmmo)
                Destroy(gameObject);
        }
    }
    /*
    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
        {
            // Boost Player Ammo

            other.GetComponent<ThrowGrenadeScript>().Reload(numAmmo);
            Destroy(gameObject);
        }
    }
    */
}
