using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirctionalTriggerBackScript : MonoBehaviour
{
    private FlashlightTriggerScript _parentScript;
    // Start is called before the first frame update
    void Start()
    {
        _parentScript = gameObject.GetComponentInParent<FlashlightTriggerScript>();
        //  In future, change to <DirectionalTriggerParentScript>
    }

    void OnTriggerEnter(Collider c)
    {
        _parentScript.BackEnter(c);
    }
    // void OnTriggerStay(Collider c)
    // {
    //     _parentScript.BackStay(c);
    // }
    void OnTriggerExit(Collider c)
    {
        _parentScript.BackExit(c);
    }

}
