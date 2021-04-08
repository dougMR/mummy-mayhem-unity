using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingDoorScript : MonoBehaviour
{
    public GameObject ignoreGO;
    // public bool useCollide = false;
    // public bool useTrigger = false;
    public bool openOnly = false;
    public bool playerOnly = true;
    public bool remoteTrigger = false;
    private Animator _animator;
    private bool _isOpen = false;

    // Start is called before the first frame update
    void Start()
    {
        _animator = transform.Find("Door").GetComponent<Animator>();
        _animator.SetBool("isOpen", false);
        // Debug.Log("SlidingDoor...");
        // Debug.Log("useCollide: " + useCollide);
        // Debug.Log("useTrigger: " + useTrigger);
    }

    /* Removed Rigidbody
    void OnCollisionEnter(Collision collision)
    {
        if (useCollide && collision.gameObject != ignoreGO)
        {
            // Debug.Log("SlidingDoor -> OnCOllisionEnter()");
            if (!_isOpen)
            {
                Open();
            }
            else
            {
                Close();
            }
        }
    }
    */
    

    void OnTriggerEnter(Collider other)
    {
        // Debug.Log(transform.name +" -> onTriggerEnter()");
        // Debug.Log("other.name:: "+other.name);
        // Debug.Log("PlayerOnly:: "+playerOnly);
        // Debug.Log("_isOpen:: "+_isOpen);
        if ( !remoteTrigger && ((playerOnly && other.name == "Player") || !playerOnly && other.gameObject != ignoreGO))
        {
            Debug.Log("SlidingDoor[ "+gameObject.name+" ] TriggerEnter other.name ==> "+other.name);
            Debug.Log("VS ignoreGO.name ==> "+ignoreGO.name);
            if (!_isOpen)
            {
                Open();
            }
        }
    }
    void OnTriggerExit(Collider other)
    {
        if ( !openOnly && ((playerOnly && other.name == "Player") || !playerOnly))
        {
            // Debug.Log("SlidingDoor -> onTriggerExit()");
            if (_isOpen)
            {
                Close();
            }
        }
    }

    // Open / Close the Door
    public void Open()
    {
        // Debug.Log("Open(" + transform.name + ")");
        _isOpen = true;
        _animator.SetBool("isOpen", true);
    }
    public void Close()
    {
        // Debug.Log("Close(" + transform.name + ")");
        _isOpen = false;
        _animator.SetBool("isOpen", false);
    }
}
