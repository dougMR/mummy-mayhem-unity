using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyFirstInventorySingletonScript : Singleton<MyFirstInventorySingletonScript>
{
    public int value = 10;
    
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
    }
}
