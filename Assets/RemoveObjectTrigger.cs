using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveObjectTrigger : MonoBehaviour

/*
    Tigger only works by Player

    This trigger removes GameObject targetGO.  This can be used for unblocking entrances, unlatching seesaws, etc.  It was created with the idea of making "Levels" accessible.  eg., once you get into the ClubHouse and get the Red Key, this trigger can unlatch the SeeSaw, giving access to the SeeSaw building, which requires the red key to get out of.

    This trigger is similar to the sliding door triggers, but for objects you want to completely remove.
*/
{
    public GameObject targetGO;
    private GameObject _player;

    private void Start()
    {
        _player = GameObject.Find("Player");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (targetGO != null && other.gameObject == _player)
        {
            // Remove the target object.
            // targetGO.SetActive(false);
            Destroy(targetGO);
            // Remove this object
            Destroy(gameObject);
        }
    }
}
