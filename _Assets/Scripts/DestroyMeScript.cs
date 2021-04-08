using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
After set time, % chance of Destroy
*/

public class DestroyMeScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (Random.Range(0, 4) == 0)
        {
            float lifetime = Random.Range(30.0f, 60.0f);
            Destroy(gameObject, lifetime);
        } else {
            float lifetime = Random.Range(5f, 20.0f);
            Destroy(gameObject, lifetime);
        }
    }
}
