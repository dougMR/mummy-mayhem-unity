using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HydrationPickupScript : MonoBehaviour
{
    public int HydrationPoints = 5;

    void OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.gameObject;
        if (other.name == "Player")
        {
            // Boost Player Hydration
            bool pickedUpHydration = other.GetComponent<HydrationScript>().Hydrate(HydrationPoints);
            if (pickedUpHydration)
            {
                Destroy(gameObject);
            }
        }
    }

}
