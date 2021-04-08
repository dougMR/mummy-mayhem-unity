using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirctionalTriggerFrontScript : MonoBehaviour
{
    private FlashlightTriggerScript _parentScript;
    // Start is called before the first frame update
    void Start()
    {
        _parentScript = gameObject.GetComponentInParent<FlashlightTriggerScript>();
        // Debug.Log("parentScript: [" + _parentScript + "]");
        //  In future, change to <DirectionalTriggerParentScript>
    }

    void OnTriggerEnter(Collider c)
    {
        _parentScript.FrontEnter(c);
    }
    // void OnTriggerStay(Collider c)
    // {
    //     _parentScript.FrontStay(c);
    // }
    void OnTriggerExit(Collider c)
    {
        _parentScript.FrontExit(c);
    }

}
