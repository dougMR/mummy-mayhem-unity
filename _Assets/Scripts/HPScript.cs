using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class HPScript : MonoBehaviour
{
    public Image damageImage;
    public Image healImage;
    public Image healthBar;
    public int maxHP;
    private int HP;
    private Coroutine damageCo;
    private Coroutine healCo;
    public AudioClip healthClip;
    private AudioSource healthSound;
    
 

    // Start is called before the first frame update
    void Start()
    {
        HP = maxHP;
        Color c = damageImage.color; 
        c.a = 0;
        damageImage.color = c;
        healImage.color = c;

        if (healthClip != null)
        {
            healthSound = gameObject.AddComponent<AudioSource>();
            healthSound.clip = healthClip;
        }

    }

    // Update is called once per frame
    void UpdateHealthBar()
    {
        float ratio = (float)HP / (float)maxHP;
        // Debug.Log("Healtbar Ratio: "+HP+"/"+maxHP+" = "+ratio);
        healthBar.fillAmount = ratio;
    }

    public void TakeDamage(int amount = 1)
    {
        HP = Mathf.Clamp(HP-amount, 0, maxHP);
        if (damageCo != null)
            StopCoroutine(damageCo);
        damageCo = StartCoroutine(FlashImage(damageImage));
        UpdateHealthBar();
    }

    public void Heal(int amount = 1) {
        HP = Mathf.Clamp(HP+amount, 0, maxHP);
        if (healCo != null)
            StopCoroutine(healCo);
        healCo = StartCoroutine(FlashImage(healImage));
        UpdateHealthBar();
        healthSound.Play();
    }

    IEnumerator FlashImage( Image whichImage )
    {
        // Debug.Log(" ----- CoRoutine START!!");
        whichImage.gameObject.SetActive(true);
        whichImage.color = Color.white;
        for (float ft = 1f; ft >= 0; ft -= 0.1f)
        {
            // Debug.Log("FlashDamageImage(" + ft + ")");
            Color c = whichImage.color;
            if (ft < 0.1f)
                ft = 0;
            
            c.a = ft;
            whichImage.color = c;
            yield return new WaitForSeconds(.1f);;
        }
        // Debug.Log(" ----- CoRoutine OVER!!");
        whichImage.gameObject.SetActive(false);
    }
}
