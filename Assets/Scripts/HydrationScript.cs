// This handles Player Hydration

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class HydrationScript : MonoBehaviour
{
    public Image damageImage;
    public Image hydrateImage;
    public Image hydrationBar;
    public Image dehydrateImage;
    public float max_hydration = 100;
    private float _hydration;
    private Coroutine damageCo;
    private Coroutine hydrateCo;
    private Coroutine dehydrateCo;
    public AudioClip hydrateClip;
    private AudioSource hydrateSound;
    public AudioClip dehydrateClip;
    private AudioSource dehydrateSound;
    private float dehydratePerMinute = 2f;
    private float nextDehydrateTime = 0.0f;
    private float dehydratePeriod = 60f;
    private int showedDehydrateMessage = 0;


    // Start is called before the first frame update
    void Start()
    {
        HydrationPoints = max_hydration * 0.5f;
        Color c = damageImage.color;
        c.a = 0;
        damageImage.color = c;
        hydrateImage.color = c;
        dehydrateImage.color = c;

        if (hydrateClip != null)
        {
            hydrateSound = gameObject.AddComponent<AudioSource>();
            hydrateSound.clip = hydrateClip;
        }
        if (dehydrateClip != null)
        {
            dehydrateSound = gameObject.AddComponent<AudioSource>();
            dehydrateSound.clip = dehydrateClip;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextDehydrateTime)
        {
            // Time to dehydrate
            nextDehydrateTime += dehydratePeriod;
            Hydrate(-1f * dehydratePerMinute);

            if (showedDehydrateMessage < 2)
            {
                showedDehydrateMessage++;
                GameManager.Instance.ShowMessage("You need to keep hydrated");
            }
        }
    }
    void UpdateHydrationBar()
    {
        float ratio = (float)HydrationPoints / (float)max_hydration;
        // Debug.Log("Hydrationbar Ratio: "+_hydration+"/"+max_hydration+" = "+ratio);
        hydrationBar.fillAmount = ratio;
    }

    public bool Hydrate(float amount = 1)
    {
        // Handles dehydration also -> pass negative amount
        if (amount > 0 && HydrationPoints == max_hydration)
        {
            // Fully Hydrated, can't hydrate more
            GameManager.Instance.ShowMessage("You're fully Hydrated");
            PlayerManager.Instance.PlayAmmoFullSound();
            return false;
        }
        if (amount > 0 && HydrationPoints < max_hydration)
        {
            // Hydrate
            if (hydrateCo != null)
                StopCoroutine(hydrateCo);
            hydrateCo = StartCoroutine(FlashImage(hydrateImage));
            UpdateHydrationBar();
            hydrateSound.Play();
        }
        else if (amount < 0)
        {
            // Dehydrate
            if (dehydrateCo != null)
                StopCoroutine(dehydrateCo);
            dehydrateCo = StartCoroutine(FlashImage(dehydrateImage));
            UpdateHydrationBar();
            dehydrateSound.Play();
            if (HydrationPoints == 0)
            {
                // Take damage
                IDamageable damageable = gameObject.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    float damage = -1f * amount;
                    damageable.TakeDamage((int)(damage * 0.5f));
                }
            }
        }
        HydrationPoints = Mathf.Clamp(HydrationPoints + amount, 0, max_hydration);

        return true;
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

    public float HydrationPoints
    {
        get => _hydration;
        set
        {
            _hydration = value;
            UpdateHydrationBar();
        }
    }
}
