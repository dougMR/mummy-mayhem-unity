using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightTriggerScript : MonoBehaviour
{
    // private Light[] _lights;
    private Light _flashlight;
    // private Light _flashlightAura;
    private Light _flashlightInner;
    private GameObject _player;
    private bool _inBack = false; // player in front collider
    private bool _inFront = false; // player in back collider

    void Start()
    {
        _flashlight = GameObject.Find("Player/Main Camera/Flashlight").GetComponent<Light>();
        // _flashlightAura = GameObject.Find("Player/Main Camera/Flashlight Aura").GetComponent<Light>();
        _flashlightInner = GameObject.Find("Player/Main Camera/Flashlight Inner").GetComponent<Light>();
        // _lights = GetComponentsInChildren(typeof(Light), true) as Light[];
        _player = GameObject.Find("Player");
        // Debug.Log("_player.name: " + _player.name);
    }

    void TurnOn()
    {
        _flashlight.enabled = _flashlightInner.enabled = true; // = _flashlightAura.enabled
        // foreach(Light light in _lights){
        //     light.enabled = true;
        // }
    }
    void TurnOff()
    {
        _flashlight.enabled = _flashlightInner.enabled = false; // =  _flashlightAura.enabled
        // foreach(Light light in _lights){
        //     light.enabled = false;
        // }
    }

    // Front Triggers
    public void FrontEnter(Collider other)
    {
        if (other.gameObject == _player)
            // Debug.Log("FrontEnter()");
        _inFront = true;

    }
    public void FrontStay(Collider other)
    {
        if (other.gameObject == _player)
            _inFront = true;
    }
    public void FrontExit(Collider other)
    {
        if (other.gameObject == _player)
        {
            // Debug.Log("FrontExit()");
            _inFront = false;
            if (!_inBack)
                TurnOn();
        }
    }

    // Back Triggers
    public void BackEnter(Collider other)
    {
        if (other.gameObject == _player)
            // Debug.Log("BackEnter()");
        _inBack = true;

    }
    public void BackStay(Collider other)
    {
        if (other.gameObject == _player)
            _inBack = true;
    }
    public void BackExit(Collider other)
    {
        if (other.gameObject == _player)
        {
            // Debug.Log("BackExit()");
            _inBack = false;
            if (!_inFront)
                TurnOff();
        }
    }
}
