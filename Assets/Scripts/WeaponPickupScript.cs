using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickupScript : MonoBehaviour
{

    public string weaponName;
    public AudioClip pickupClip;
    private AudioSource _pickupSound;
    void Start()
    {
        _pickupSound = gameObject.AddComponent<AudioSource>();
        _pickupSound.clip = pickupClip;

    }
    void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
        {
            // Pick Up Weapon
            _pickupSound.Play();
            PlayerManager.Instance.AddWeaponByName(weaponName);

            Destroy(gameObject, 1f);
            // 1.0f delay and all below, just so audio can play before object is destroyed.
            MeshRenderer[] meshes = GetComponentsInChildren<MeshRenderer>(false);
            foreach(MeshRenderer mesh in meshes ){
                mesh.enabled = false;
            }
            GetComponent<CapsuleCollider>().enabled = false;
            GetComponent<ParticleSystem>().Stop();
        }
    }
}
