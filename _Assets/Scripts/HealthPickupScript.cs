using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickupScript : MonoBehaviour
{
    public int HP = 5;

    void OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.gameObject;
        if (other.name == "Player")
        {
            // Boost Player HP

            other.GetComponent<HPScript>().Heal(HP);
            Destroy(gameObject);
        }
    }
    /*
    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
        {
            // Boost Player HP

            other.GetComponent<HPScript>().Heal(HP);
            Destroy(gameObject);
        }
    }
    */
}
