using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeCollideScript : MonoBehaviour
{
    public GameObject explosionGO; // drag your explosion prefab here
    public GameObject explosionGO2;
    private float radius = 12;
    private float explodePower = 10;
    // private float upwardsModifier = 1.0F;
    private GameObject explosionParticleEffect;

    private bool detonated = false;

    void Start()
    {
        // explosionGO = Resources.Load("Explosion") as GameObject;
    }

    /*
    void OnTriggerEnter(Collider other){
        Debug.Log("Grenade collided.");
        
        // if (!other.CompareTag("Player"))
        // {
        if (((1 << other.gameObject.layer) & LayerMask.GetMask("Player")) == 0)
        {
            //it isnt in the layermask
            
            Debug.Log("Grenade Hit - "+other.name);
            // Vector3 cp = other.ClosestPoint(transform.position);
            // transform.position = cp;
            GameManager.Instance.GamePaused = true;
            Detonate();
        }
    }
    */



    void OnCollisionEnter(Collision collision)
    {

        // Debug.Log("Grenade collided.");
        GameObject other = collision.gameObject;

        if (!other.CompareTag("Player"))
        {
            // Debug.Log("Grenade Hit "+collision.collider.name);
            // GameManager.Instance.GamePaused = true;

            // int numContacts = collision.contactCount;
            // ContactPoint cp = collision.GetContact(0);
            // transform.position = cp.point + cp.normal * 0.1f;
            Detonate();
        }
    }



    private void Detonate()
    {
        if (!detonated)
        {
            detonated = true;
            // Debug.Log("Grenade.Detonate()");

            // Remove this object from interfering with explosion force
            Destroy(gameObject);

            gameObject.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

            gameObject.GetComponent<Rigidbody>().isKinematic = true;

            GameManager.Instance.Explode(transform.position, radius, explodePower, explosionGO, "Enemies");

            // Show 2nd explosion animation
            GameObject expl = (GameObject)Instantiate(explosionGO2, transform.position, Quaternion.identity);

        }
    }
}
