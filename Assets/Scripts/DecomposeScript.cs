using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecomposeScript : MonoBehaviour

/* Disconnect children from parent object, and destroy parent */
{
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        // Decompose();
    }

    public void Decompose() {
        rb.detectCollisions = false;
        Unnest(transform);
        Destroy(transform.gameObject);
    }
    void Unnest(Transform parentTransform)
    {
        Debug.Log("Parent: " + parentTransform);
        Debug.Log("#children: " + parentTransform.childCount);
        // parentTransform[] children = parentTransform[parentTransform.childCount];
        Transform[] children = parentTransform.gameObject.GetComponentsInChildren<Transform>();
        
        foreach (Transform child in children)
        {
            // Debug.Log("Foreach loop: " + child);
            if (child != parentTransform)
            {
                Animator anim = child.gameObject.GetComponent<Animator>();
                if ( anim != null ) {
                    anim.enabled = false;
                }
                // childObject.transform.parent.gameObject
                child.SetParent(null);
                Collider childCollider = child.gameObject.GetComponent<Collider>();
                Debug.Log(childCollider);
                childCollider.enabled = true;
                Rigidbody gameObjectsRigidBody = child.gameObject.AddComponent<Rigidbody>(); // Add the rigidbody.

                // child.attachedRigidbody.useGravity = true;
                // Debug.Log("Collider.enabled = " + childCollider.enabled);
                // Destroy(child.gameObject);
                // Debug.Log("#children: " + parentTransform.childCount);
            }
        }
    }
}