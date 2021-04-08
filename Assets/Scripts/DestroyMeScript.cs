using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
After set time, % chance of Destroy
*/

public class DestroyMeScript : MonoBehaviour
{
    public float delay = 0.0f;

    private float _minShortLife = 1.0f;
    private float _maxShortLife = 10.0f;
    private float _minLongLife = 5.0f;
    private float _maxLongLife = 30.0f;
    private float _lifetime;

    // Start is called before the first frame update
    void Start()
    {
        if(delay > 0f)
        {
            _lifetime = delay;
        
        }else{
            // For Mummy Debris
            if (Random.Range(0, 4) == 0)
            {
                _lifetime = Random.Range(_minLongLife, _maxLongLife);
            } else {
                _lifetime = Random.Range(_minShortLife, _maxShortLife);
            }
        }
        Destroy(gameObject, _lifetime);
    }
}
