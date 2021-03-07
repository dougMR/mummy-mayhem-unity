using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommotionScript : MonoBehaviour
{
    public float duration = 1f;
    public float radius;
    // Start is called before the first frame update
    void Start()
    {
        Radius = radius;
        Duration = duration;
    }

    private void OnTriggerStay(Collider other)
    {
        GameObject otherGO = other.gameObject;
        if (IsMummy(otherGO))
        {
            otherGO.GetComponent<EnemyAIScript>().FaceOther(gameObject);
        }
    }
    private bool IsMummy(GameObject other)
    {
        return other.name.Contains("Mummy") && !other.name.Contains("Separated");
    }
    public float Radius
    {
        get { return radius; }
        set
        {
            radius = value;
            // transform.Find("Sphere").transform.localScale = new Vector3(radius*2, radius*2, radius*2);
            // transform.GetComponent<SphereCollider>().radius = Radius;
        }
    }
    public float Duration
    {
        get { return duration; }
        set
        {
            duration = value;
            CancelInvoke("DestroySelf");
            Invoke("DestroySelf", duration);
        }
    }

    void DestroySelf()
    {
        Destroy(gameObject);
    }
}
