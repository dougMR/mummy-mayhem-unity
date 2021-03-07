using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverSwitchScript : MonoBehaviour
{
    public GameObject switchTarget;
    public string targetBoolString;
    private Animator _animator = null;
    private Animator _targetAnimator = null;
    private bool _isOn = false;
    private bool _playerClose = false;
    private GameManager GM;


    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _targetAnimator = switchTarget.GetComponent<Animator>();
        GM = GameManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        // Is User pressing E key?
        if (_playerClose && Input.GetKeyUp(KeyCode.E))
        {
            FlipSwitch();
        }
    }
    void OnTriggerEnter(Collider other)
    {
        // Is Player in my Trigger?
        if (other.name == "Player")
        {
            _playerClose = true;
            GM.ShowMessage("Press 'E' to switch Lever.");
        }
    }
    void OnTriggerExit(Collider other)
    {
        // Is Player in my Trigger?
        if (other.name == "Player")
        {
            _playerClose = false;
        }
    }

    private void FlipSwitch()
    {
        Switch(!_isOn);
    }
    // Switch On / Off
    public void Switch(bool turnOn)
    {
        _isOn = turnOn;
        Debug.Log("Switch (" + transform.name + ") switching to " + _isOn);
        _animator.SetBool("isOn", _isOn);
        _targetAnimator.SetBool(targetBoolString, _isOn);
        Debug.Log(_targetAnimator + " switchTarget.name " + _targetAnimator.GetBool(targetBoolString));
    }

}
