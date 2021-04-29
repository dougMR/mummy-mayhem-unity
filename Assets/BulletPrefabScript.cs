using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPrefabScript : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        // Pause Game
        // GameManager.Instance.GamePaused = true;
        // Destroy me
        Destroy(gameObject);
    }
}
