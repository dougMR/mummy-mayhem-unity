// This handles Player HP

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // For restarting scene

using UnityEngine.UI;

public class HPScript : MonoBehaviour, IDamageable
{
    public Image damageImage;
    public Image healImage;
    public Image healthBar;
    public int max_hp;
    private int _hp;
    private Coroutine damageCo;
    private Coroutine healCo;
    public AudioClip healthClip;
    private AudioSource healthSound;



    // Start is called before the first frame update
    void Start()
    {
        HP = max_hp;
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
        float ratio = (float)HP / (float)max_hp;
        // Debug.Log("Healtbar Ratio: "+_hp+"/"+max_hp+" = "+ratio);
        healthBar.fillAmount = ratio;
    }

    public void TakeDamage(int amount = 1)
    {
        Debug.Log("HPScript.TakeDamage(" + amount + ")");
        HP = Mathf.Clamp(HP - amount, 0, max_hp);
        if (damageCo != null)
            StopCoroutine(damageCo);
        damageCo = StartCoroutine(FlashImage(damageImage));
        UpdateHealthBar();
        // CHECK IF DEAD !!
        // if (HP <= 0)
        // {
        //     GameManager.Instance.DelayFunction(() =>
        // {
        //     SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        // }, 2f);
        // }
    }

    public void Heal(int amount = 1)
    {
        HP = Mathf.Clamp(HP + amount, 0, max_hp);
        if (healCo != null)
            StopCoroutine(healCo);
        healCo = StartCoroutine(FlashImage(healImage));
        UpdateHealthBar();
        healthSound.Play();
    }

    IEnumerator FlashImage(Image whichImage)
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
            yield return new WaitForSeconds(.1f); ;
        }
        // Debug.Log(" ----- CoRoutine OVER!!");
        whichImage.gameObject.SetActive(false);
    }

    public int HP
    {
        get => _hp;
        set
        {
            _hp = value;
        }
    }
}
