using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketFlyScript : MonoBehaviour
{

    private Rigidbody rb;
    private float thrust = 20.0f;
    public GameObject explosionGO; // drag your explosion prefab here
    private float radius = 12;
    private float explodePower = 400;
    private float upwardsModifier = 1.0F;
    // private GameObject explosionParticleEffect;
    private float _lifetime = 8.0f;
    // private bool detonated = false;
    // Start is called before the first frame update
    private CauseCommotionScript _commotionScript;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddTorque(transform.forward * (-3));
        rb.velocity = transform.forward * thrust;
        _commotionScript = GetComponent<CauseCommotionScript>();
    }

    void FixedUpdate()
    {
        rb.AddForce(transform.forward * thrust);
        rb.AddForce(-transform.up * 1.0f);
        _lifetime -= Time.deltaTime;
        if (_lifetime <= 0)
            Detonate();
    }

    void OnCollisionEnter(Collision collision)
    {
        // Debug.Log("Grenade collided.");
        GameObject other = collision.gameObject;

        if (!other.CompareTag("Player"))
        {
            int numContacts = collision.contactCount;
            ContactPoint cp = collision.GetContact(0);
            // transform.position = cp.point;
            Detonate();
        }
    }

    void Detonate()
    {

        // CHange this to use GameManager.Instance.Explode

        // if (!detonated)
        // {
        // detonated = true;

        // Apply explosion force to nearby objects
        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
        for (int i = 0; i < colliders.Length; i++)
        {
            Rigidbody rb = colliders[i].GetComponent<Rigidbody>();

            if (rb != null && !rb.CompareTag("Player") && rb != gameObject.GetComponent<Rigidbody>())
            {

                bool exposed = true;
                RaycastHit hit;
                Vector3 dir = (explosionPos - rb.position).normalized;
                float dist = Vector3.Distance(rb.position, explosionPos);
                // Does the ray intersect any objects excluding the rb
                // Does the ray intersect any objects excluding the player layer
                if (Physics.Raycast(rb.transform.position, dir, out hit, dist))
                {
                    exposed = rb == hit.rigidbody || hit.transform.name.Contains("Rocket") || hit.transform.name.Contains("Ground");
                }
                if (exposed)
                {

                    if (rb.CompareTag("Mummy"))
                    {
                        Rigidbody[] children = rb.GetComponent<SwapMummyScript>().Swap().GetComponentsInChildren<Rigidbody>();

                        foreach (Rigidbody child in children)
                        {
                            if (child != null)
                            {
                                if (Random.Range(0, 4) == 0)
                                {
                                    // Debug.Log("GrenadeCollide - Torso");
                                    List<GameObject> pieces = child.GetComponent<SubdivideObjectScript>().SubdivideMe();
                                    // Debug.Log("pieces.Length: "+pieces.Length);
                                    for (int p = 0; p < pieces.Count; p++)
                                    {

                                        GameObject piece = pieces[p];
                                        // Debug.Log("piece["+p+"]: "+piece);
                                        piece.GetComponent<Rigidbody>().AddExplosionForce(explodePower, explosionPos, radius, upwardsModifier);
                                    }
                                }
                                else
                                {
                                    child.AddExplosionForce(explodePower, explosionPos, radius, upwardsModifier);
                                }
                            }
                        }

                    }
                    else if (rb.name == "Head" || rb.name == "Torso" || rb.name == "Legs")
                    {
                        List<GameObject> pieces = rb.GetComponent<SubdivideObjectScript>().SubdivideMe();
                        // Debug.Log("pieces.Length: "+pieces.Length);
                        for (int p = 0; p < pieces.Count; p++)
                        {

                            GameObject piece = pieces[p];
                            // Debug.Log("piece["+p+"]: "+piece);
                            piece.GetComponent<Rigidbody>().AddExplosionForce(explodePower, explosionPos, radius, upwardsModifier);
                        }
                    }
                    else
                    {
                        rb.AddExplosionForce(explodePower, explosionPos, radius, upwardsModifier);
                    }
                }

            }

        }

        // show explosion
        GameObject expl = (GameObject)Instantiate(explosionGO, transform.position, transform.rotation);

        Destroy(expl, 3); // delete the explosion after 3 seconds

        _commotionScript.CauseCommotion(50f, 3f);

        Destroy(gameObject);

    }
}
