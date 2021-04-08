using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CauseCommotionScript : MonoBehaviour
{
    public GameObject commotionPrefab;

    public void CauseCommotion(float radius, float duration)
    {
        GameObject commotion = Instantiate(commotionPrefab, transform.position, Quaternion.identity);
        commotion.GetComponent<CommotionScript>().Duration = duration;
        commotion.GetComponent<CommotionScript>().Radius = radius;
    }
}
