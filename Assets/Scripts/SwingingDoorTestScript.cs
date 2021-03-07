using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingingDoorTestScript : MonoBehaviour
{
    public AudioClip doorCreakClip;
    public string myKeyName;
    private Animator _animator = null;
    private bool _isOpen = false;
    private AudioSource _doorCreakSound;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();

        _doorCreakSound = gameObject.AddComponent<AudioSource>();
        _doorCreakSound.clip = doorCreakClip;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other) {
        
        if(!_isOpen && other.name == "Player"){
            // If Player has the Key
            if ( InventoryManager.Instance.HasItemByName(myKeyName)){
                InventoryItem myKey = InventoryManager.Instance.GetItemByName(myKeyName);
                myKey.ShowSprite();
                _animator.SetBool("open", true);
                _isOpen = true;
                InventoryManager.Instance.RemoveItem(myKey);
                _doorCreakSound.Play();
            } else {
                GameManager.Instance.ShowMessage("You need a "+myKeyName);
            }
        }        
    }
    // private void OnTriggerExit(Collider other) {
    //     // _animator.SetBool("open", false);
    // }
}
