using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapMummyScript : MonoBehaviour
{

    public GameObject separatedMummy;
    private float lastVelocity;
    // private float maxVelocityChange = 2f;
    // Start is called before the first frame update
    void Start()
    {
        // Swap();
        // lastVelocity = gameObject.GetComponent<Rigidbody>().velocity.magnitude;
    }

/*
    void FixedUpdate()
    {
        float newVelocity = gameObject.GetComponent<Rigidbody>().velocity.magnitude;

        if (Mathf.Abs(newVelocity - lastVelocity) > maxVelocityChange)
        {
            Debug.Log("V: " + Mathf.Abs(newVelocity - lastVelocity));
            // Swap();
        }
        else
        {
            lastVelocity = newVelocity;
        }

    }
*/  

    public GameObject Swap()
    {
        
        Debug.Log("Swap() "+gameObject.name);
        if(gameObject.GetComponent<Collider>().enabled == false){
            Debug.Log("Swap collider.enabled ==> "+gameObject.GetComponent<Collider>().enabled);
            return null;
        }
        gameObject.GetComponent<MummyGroanScript>().StopGroan();
        gameObject.GetComponent<Collider>().enabled = false;
        gameObject.GetComponent<Rigidbody>().isKinematic = true;

        GameObject swappedMummy = Instantiate(separatedMummy, transform.position, transform.rotation);
        Debug.Log("SwapMummyScript swappedMummy.transform.position.y: "+swappedMummy.transform.position.y);

        Rigidbody[] children = swappedMummy.GetComponentsInChildren<Rigidbody>();
        // Juggle them around a little
        foreach ( Rigidbody child in children ) {
            Vector3 pos = child.transform.position;
            pos.x += -0.5f + Random.Range(0.0f, 1f);
            pos.z += -0.5f + Random.Range(0.0f, 1f);;
            child.transform.position = pos;
        }

        Destroy(gameObject);                         
        return swappedMummy;
    }
}
