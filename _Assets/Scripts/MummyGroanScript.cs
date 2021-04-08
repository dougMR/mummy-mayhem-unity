using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MummyGroanScript : MonoBehaviour
{
    private AudioSource groanSound;
    private AudioSource vocalGroan;
    // private AudioSource[] aSources;

    // private AudioClip groanClip;
    // Start is called before the first frame update
    void Start()
    {
        AudioSource[] aSources = GetComponents<AudioSource>();
        groanSound = aSources[0];

        float clipLength = Mathf.Floor(groanSound.clip.length);
        // Debug.Log("groansound length: " + groanSound.clip.length);
        float groanDelay = Random.Range(0, clipLength);
        // groanSound.Stop();
        groanSound.loop = true;
        // groanSound.PlayDelayed(groanDelay);
        groanSound.time = groanDelay;
        groanSound.mute = false;
        groanSound.Play();
/*
        vocalGroan = aSources[1];
        if(vocalGroan != null) {
            // vocalGroan.volume = 0.5f;
            groanDelay = Random.Range(0, Mathf.Floor(vocalGroan.clip.length));
            vocalGroan.time = groanDelay;
            vocalGroan.loop = true;
            vocalGroan.Play();
        }
*/
    }

    public void StopGroan(){
        // Debug.Log("StopGroan()"+groanSound);
        if (groanSound == null ){
            return;
        }
        groanSound.loop = false;
        groanSound.mute = true;
        groanSound.Stop();
    }
}
