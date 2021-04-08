using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerButtonScript : MonoBehaviour
{
    public GameObject myDevice;
    private bool isOn = true;
    private Renderer myRenderer;

    void Start() {
        myRenderer = gameObject.GetComponent<Renderer>();
        myRenderer.material.SetColor( "_EmissionColor", Color.green );
    }

    // When player collides w/ button, toggle Spawner on / off.
    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
        {
            Switch();
        }
    }

    private void Switch()
    {
        Debug.Log("Switch Mummy Spawner");

        isOn = !isOn;
        Debug.Log("isOn: "+isOn);
        
        if (isOn)
        {
            myDevice.GetComponent<SpawnerScript>().TurnOn();
        }
        else
        {
            myDevice.GetComponent<SpawnerScript>().TurnOff();
        }
        // myRenderer.material.color = isOn ? Color.green : Color.red;
        myRenderer.material.SetColor( "_EmissionColor", isOn ? Color.green : Color.red);
    }
}
