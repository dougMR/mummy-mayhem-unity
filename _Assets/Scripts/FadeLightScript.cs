using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeLightScript : MonoBehaviour
{
    private Light lightToFade;

    public float inDur = 1f;
    public float outDur = 1f;
    public float delay = 0f;

    void Start()
    {
        lightToFade = gameObject.GetComponent<Light>();
        lightToFade.intensity = 0;
        StartCoroutine(fadeInAndOutRepeat(lightToFade, inDur, outDur, delay, 1));
    }

    //Fade in and out forever
    IEnumerator fadeInAndOutRepeat(Light lightToFade, float inDur, float outDur, float delay, int loops = 1)
    {
        // Debug.Log("FadeInAndOutRepeat");
        WaitForSeconds waitForXSec = new WaitForSeconds(delay);

        while (loops > 0)
        {
            //Wait
            yield return waitForXSec;

            //Fade out
            yield return fadeInOrOut(lightToFade, true, inDur);

            //Fade-in 
            yield return fadeInOrOut(lightToFade, false, outDur);
            
            loops--;
        }
    }

    IEnumerator fadeInOrOut(Light lightToFade, bool fadeIn, float duration)
    {
        // string dir = fadeIn ? "In" : "Out";
        // Debug.Log("Fade -"+dir+"-");
        float minLuminosity = 0; // min intensity
        float maxLuminosity = 3; // max intensity

        float counter = 0f;

        //Set Values depending on if fadeIn or fadeOut
        float a, b;

        if (fadeIn)
        {
            a = minLuminosity;
            b = maxLuminosity;
        }
        else
        {
            a = maxLuminosity;
            b = minLuminosity;
        }

        float currentIntensity = lightToFade.intensity;

        while (counter < duration)
        {
            counter += Time.deltaTime;

            lightToFade.intensity = Mathf.Lerp(a, b, counter / duration);

            yield return null;
        }
        lightToFade.intensity = 0;
    }
}
