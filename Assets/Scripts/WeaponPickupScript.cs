using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickupScript : MonoBehaviour
{

    public string weaponName;
    // public AudioClip pickupClip;
    // private AudioSource _pickupSound;
    void Start()
    {
        // _pickupSound = gameObject.AddComponent<AudioSource>();
        // _pickupSound.clip = pickupClip;

    }
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("WeaponPickupScript :: "+weaponName);
        if (other.name == "Player")
        {
            if (  PlayerManager.Instance.AddWeaponByName(weaponName) ){
                // Pick Up Weapon
                // _pickupSound.Play();
                // PlayerManager.Instance.Audio.PlayOneShot(PlayerManager.Instance.GetWeaponByName(weaponName).LoadClip);
                // Destroy pickup
                Destroy(gameObject);
                // 1.0f delay and all below, just so audio can play before object is destroyed.
                /*
                MeshRenderer[] meshes = GetComponentsInChildren<MeshRenderer>(false);
                foreach(MeshRenderer mesh in meshes ){
                    mesh.enabled = false;
                }
                GetComponent<CapsuleCollider>().enabled = false;
                GetComponent<ParticleSystem>().Stop();
                */
            }
        }
    }
}
