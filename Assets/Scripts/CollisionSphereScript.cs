using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSphereScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // GameManager.Instance.GamePaused = true;
    }

    private void OnCollisionEnter(Collision other)
    {
        if(other.collider.name != "Player" && !other.transform.IsChildOf(GameObject.Find("Player").transform)){
            // float estFrameDuration = Time.deltaTime;
            Destroy(gameObject);
            // this.GetComponent<Rigidbody>().isKinematic = true;
            // this.GetComponent<SphereCollider>().enabled = false;
        }
        
    }
}
