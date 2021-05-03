﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEditor;

public class GameManager : Singleton<GameManager>
{
    public Text HUDTextField;
    public Text scoreText;
    // public Text OutputField;
    public GameObject pausedScreen;
    public GameObject controlsText;
    public GameObject commotionPrefab;
    private int _score = 0;
    private Coroutine _messageCo = null;
    private Coroutine _messageZoomCo = null;
    // private bool _messageCoRunning = false;
    private Coroutine _spriteCo;
    private static bool _gameIsPaused;
    private Image _spriteHolderImage;
    private string _lastMessage = "";
    private float _lastMessageTime = 0f;
    private float _timeRemainingInCurrentMessage = 0f;
    private float _defaultMessageDuration = 2f;
    private GameObject _player;
    private int _difficultyLevel = 1;
    private int _enemiesMask = -1;

    protected override void Awake()
    {
        base.Awake();
        HUDTextField.gameObject.SetActive(false);
        pausedScreen.SetActive(false);
        _spriteHolderImage = GameObject.Find("UI_Canvas/SpriteHolderImage").GetComponent<Image>();
        _spriteHolderImage.gameObject.SetActive(false);
        // Debug.Log("_spriteHolderImage:: "+_spriteHolderImage);
        _player = GameObject.Find("Player");
        int layerInt = LayerMask.NameToLayer("Enemies"); // -1 if not found
        _enemiesMask = layerInt == -1 ? ~0 : 1 << layerInt; // ~ is bitwise NOT, so ~0 means All Layers
    }
    void Start()
    {
        StartGame();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            GamePaused = !GamePaused;
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            // SHow Last Message
            ShowLastMessage();
        }
    }
    public void StartGame()
    {
        // Debug.Log("    StartGame unscaledDeltaTime: " +Time.realtimeSinceStartup);
        float messageTime = ShowMessage("Lost and wandering the desert, \r\nyou come upon strange ruins...");
        // Debug.Log("    StartGame unscaledDeltaTime: " +Time.realtimeSinceStartup);
        PauseFor(messageTime);
        // Debug.Log("    StartGame unscaledDeltaTime: " +Time.realtimeSinceStartup);
        Cursor.lockState = CursorLockMode.Confined; // <- doesn't work on Mac :(
    }

    public void PauseFor(float secs = 1f)
    {
        GamePaused = true;
        StartCoroutine(UnpauseIn(secs));
    }
    IEnumerator UnpauseIn(float secs = 1f)
    {
        // Debug.Log("==========================\n\rUnpauseIn( "+secs+", starting at "+Time.realtimeSinceStartup+" )");
        float startPauseTime = Time.realtimeSinceStartup;
        yield return new WaitForSecondsRealtime(secs);
        GamePaused = false;
        // Debug.Log("UnpauseIn duration = "+(Time.realtimeSinceStartup - startPauseTime)+", ending at (unscaled)"+Time.unscaledTime+" vs (scaled)"+Time.time);
    }
    public void DelayFunction(System.Action callback, float secs)
    {
        StartCoroutine(DelayFunctionCo(callback, secs));
    }
    IEnumerator DelayFunctionCo(System.Action callback, float secs)
    {
        yield return new WaitForSeconds(secs);
        callback();
    }

    /* 
    Doing this to get some experience with Singletons and global methods. Perhaps a little hacky usage for now. 

    Put all my "Global" methods below...
    */
    // public string GetSpritePath( Sprite sprite ) {
    //     string spritePath = AssetDatabase.GetAssetPath(sprite);
    //     int indexStart = spritePath.IndexOf("Resources/") + 10;
    //     int indexEnd = spritePath.IndexOf(".");
    //     int length = indexEnd - indexStart;
    //     spritePath = spritePath.Substring(indexStart, length);
    //     return spritePath;
    // }
    public void ShowLastMessage()
    {
        // Debug.Log("ShowLastMessage()");
        ShowMessage(_lastMessage);
    }

    public float ShowMessage(string message, float delay = 0f)
    {
        // Don't show this message if the same message has been shown in the last [5] seconds

        if (message == _lastMessage && Time.time - _lastMessageTime < 5f)
        {
            return 0;
        }

        // If a message is already showing, wait until it is done to show this one

        // pause delay seconds

        if (delay > 0f || _timeRemainingInCurrentMessage > 0f)
        {

            // Debug.Log("message delay:: "+delay);
            delay = delay > 0f ? delay : _timeRemainingInCurrentMessage;
            StartCoroutine(DelayMessage(message, delay));
            return 0;
        }
        // set duration by string length of message
        float duration = 1f + message.Length / 30f;
        // Debug.Log("duration = 1f + "+message.Length+" / 30 = "+duration);
        _lastMessage = message;
        _lastMessageTime = Time.time;
        Color fullColor = HUDTextField.color;
        Color outlineColor = HUDTextField.GetComponent<Outline>().effectColor;
        fullColor.a = 1.0f;
        outlineColor.a = 0.6f;
        HUDTextField.color = fullColor;
        HUDTextField.GetComponent<Outline>().effectColor = outlineColor;
        HUDTextField.gameObject.SetActive(true);
        HUDTextField.text = message;

        if (_messageCo != null)
        {
            // Debug.Log("_messageCo != null");
            StopCoroutine(_messageCo);
        }
        if (_messageZoomCo != null)
        {
            // Debug.Log("_messageZoomCo != null");
            StopCoroutine(_messageZoomCo);
        }
        _messageZoomCo = StartCoroutine(ZoomMessageIn(duration));
        _messageCo = StartCoroutine(HideMessage(duration));
        // Debug.Log("End ShowMessage()");
        return duration;
    }

    IEnumerator DelayMessage(string message, float delay)
    {

        // Debug.Log("DelayMessage( "+delay+" )");

        yield return new WaitForSeconds(delay);
        ShowMessage(message);
    }

    IEnumerator ZoomMessageIn(float dur = 2f)
    {
        Debug.Log("GM.ZoomMessageIn()");
        // Zoom Message for dur seconds
        float startZ = 50f;
        float endZ = 0f;
        RectTransform HUDTextRectTransform = HUDTextField.GetComponent<RectTransform>();
        float X = HUDTextRectTransform.anchoredPosition3D.x;
        float Y = HUDTextRectTransform.anchoredPosition3D.y;
        float Z = startZ;
        HUDTextRectTransform.anchoredPosition3D = new Vector3(X, Y, Z);

        // zoom in
        float counter = 0f;
        while (counter < dur)
        {
            // Debug.Log("HUDTextRectTransform.anchoredPosition3D:: " + HUDTextRectTransform.anchoredPosition3D);
            counter += Time.unscaledDeltaTime;
            Z = Mathf.Lerp(startZ, endZ, counter / dur);
            //Debug.Log("zoom Z:: " + Z);
            //Debug.Log("zoom-in counter/dur : " + counter + " / " + dur + " = " + (counter / dur));
            HUDTextRectTransform.anchoredPosition3D = new Vector3(X, Y, Z);
            yield return null;
        }
    }
    IEnumerator HideMessage(float dur = 2f)
    {
        // Hide Message after dur seconds
        // Debug.Log("HideMessage after "+dur+" seconds");

        float fadeDuration = 0.5f;
        _timeRemainingInCurrentMessage = dur + fadeDuration;
        // float startWaitTime = Time.realtimeSinceStartup;
        // Debug.Log("start Hidemessage start at "+startWaitTime);
        int loops = 0;
        while (_timeRemainingInCurrentMessage > fadeDuration) //While there is still time
        {
            // Debug.Log("HideMessage counting down... "+Time.deltaTime+" vs "+Time.unscaledDeltaTime);
            _timeRemainingInCurrentMessage -= Time.unscaledDeltaTime; //Reduce the timer with time between previous and actual frame
            // Debug.Log("["+(loops++) +"] Time.unscaledDeltaTime : "+Time.unscaledDeltaTime);
            // Debug.Log("_timeRemainingInCurrentMessage ==> "+_timeRemainingInCurrentMessage);
            yield return null; // Wait for this frame to end
        }
        // Debug.Log("end Hidemessage at "+Time.realtimeSinceStartup);
        // Debug.Log("Time Elapsed : "+(Time.realtimeSinceStartup - startWaitTime));

        // fade out
        // Debug.Log("HideMessage Fadeout Start: "+Time.realtimeSinceStartup);
        RectTransform HUDTextRectTransform = HUDTextField.GetComponent<RectTransform>();
        float X = HUDTextRectTransform.anchoredPosition3D.x; //HUDTextField.transform.localPosition.x;
        float Y = HUDTextRectTransform.anchoredPosition3D.y;
        float startZ = HUDTextRectTransform.anchoredPosition3D.z;
        float Z = startZ;
        HUDTextRectTransform.anchoredPosition3D = new Vector3(X, Y, Z);

        Color currentColor = HUDTextField.color;
        Outline outline = HUDTextField.GetComponent<Outline>();
        Color outlineColor = outline.effectColor;
        float alpha = 1;

        // Debug.Log("HUDTextRectTransform.anchoredPosition3D:: "+HUDTextRectTransform.anchoredPosition3D);
        // HUDTextField.transform.position = new Vector3(0,0,Z);
        // 
        float counter = 0f;
        while (counter < fadeDuration)
        {
            counter += Time.deltaTime;

            alpha = Mathf.Lerp(1, 0, counter / fadeDuration);
            Z = Mathf.Lerp(startZ, 500f, counter / fadeDuration);
            // Debug.Log("Z: "+Z);
            HUDTextRectTransform.anchoredPosition3D = new Vector3(X, Y, Z);
            // Debug.Log("HUDTextRectTransform.anchoredPosition3D:: "+HUDTextRectTransform.anchoredPosition3D);
            currentColor.a = alpha;
            outlineColor.a = alpha * 0.6f;
            HUDTextField.color = currentColor;
            outline.effectColor = outlineColor;

            _timeRemainingInCurrentMessage -= Time.deltaTime;

            yield return null;
        }
        // HUDTextRectTransform.anchoredPosition3D = new Vector3(X, Y, 0f);
        HUDTextField.gameObject.SetActive(false);
        // _messageCoRunning = false;
        _timeRemainingInCurrentMessage = 0f;
        // Debug.Log("HideMessage Fadeout Done, at "+Time.realtimeSinceStartup);
    }

    public void ShowSprite(Sprite sprite, float duration = 0.5f)
    {
        // Debug.Log("GameManager.ShowSprite()");
        // Debug.Log("sprite: "+sprite);
        Color fullColor = _spriteHolderImage.color;
        fullColor.a = 1f;
        _spriteHolderImage.color = fullColor;
        _spriteHolderImage.gameObject.SetActive(true);
        float limitH = 400f;
        float limitW = 600f;
        float limitRatio = limitW / limitH;
        float spriteW = sprite.rect.width;
        float spriteH = sprite.rect.height;
        float spriteRatio = spriteW / spriteH;
        float scale = 1;
        if (limitRatio > spriteRatio)
        {
            // sprite Ratio is narrower than limit Ratio, so...
            scale = limitH / spriteH;
        }
        else
        {
            // sprite ratio is wider, so...
            scale = limitW / spriteW;
        }
        _spriteHolderImage.rectTransform.sizeDelta = new Vector2(spriteW * scale, spriteH * scale);
        _spriteHolderImage.sprite = sprite;
        if (_spriteCo != null)
            StopCoroutine(_spriteCo);
        _spriteCo = StartCoroutine(HideSprite(duration));
    }
    IEnumerator HideSprite(float dur = 0.5f)
    {
        // Display Sprite for dur seconds
        yield return new WaitForSeconds(dur);
        // fade out
        Color currentColor = _spriteHolderImage.color;
        float alpha = 1;
        float fadeDuration = 0.25f;
        float counter = 0f;
        while (counter < fadeDuration)
        {
            counter += Time.deltaTime;

            alpha = Mathf.Lerp(1, 0, counter / fadeDuration);
            currentColor.a = alpha;
            _spriteHolderImage.color = currentColor;

            yield return null;
        }

        _spriteHolderImage.gameObject.SetActive(false);
    }

    // public void Output(string txt, bool add = false)
    // {
    //     string prevTxt = add ? OutputField.text + "\n\r" : "";
    //     OutputField.text = prevTxt + txt;
    // }

    public void Explode(Vector3 explosionPos, float radius, float explosionForce, GameObject explosionGO, string maskLayerName = "Enemies")
    {
        Debug.Log("GameManager.Explode()");

        float upwardsModifier = 1.0F;

        // int layerInt = LayerMask.NameToLayer(maskLayerName); // -1 if not found
        // int layerMask = layerInt == -1 ? ~0 : 1 << layerInt; // ~ is bitwise NOT, so ~0 means All Layers

        int playerLayer = 1 << LayerMask.NameToLayer("Player");
        // layerMask = layerMask | playerLayer;
        int layerMask = ~0;


        // Check for Explosion hit Mummies
        Collider[] colliders = Physics.OverlapSphere(explosionPos, radius, layerMask);
        List<Collider> usedColliders = new List<Collider>(); // <-- for treating multiple GO colliders as single collider

        // Debug.Log("GM.Explode #colliders ==> " + colliders.Length);

        for (int i = 0; i < colliders.Length; i++)
        {
            Collider coll = colliders[i];

            // Check against multiple colliders in same gameObject
            if (usedColliders.Contains(coll))
            {
                // skip this collider
                // Debug.Log("Skip This Collider!! -- " + coll.name);
                continue;
            }
            usedColliders.AddRange(coll.GetComponentsInChildren<Collider>());

            Rigidbody rb = coll.GetComponent<Rigidbody>();

            if (rb != null)
            {
                // Debug.Log("GameManager.Explode i :: " + i + " :: " + rb.name);

                // Does the ray intersect any objects excluding the rb
                bool exposed = true;

                Vector3 dir = (rb.position - explosionPos).normalized;
                if (dir.magnitude == 0) dir = Vector3.up;

                // Get dist to point of intersection
                RaycastHit hit;
                Ray ray = new Ray(explosionPos, dir);

                // Debug.Log("GM.Explode (rb.position - explosionPos) : " + (rb.position - explosionPos));
                // Debug.Log("GameManager.Explode Dir (" + dir + ") vs ray.direction (" + ray.direction + ")");

                float dist;
                if (coll.Raycast(ray, out hit, radius))
                {
                    // Distance to point of ray intersect collider
                    dist = hit.distance;
                }
                else
                {
                    // Distance to rigidbody position
                    dist = Mathf.Clamp(Vector3.Distance(rb.position, explosionPos), 0, radius);
                }

                // Does the ray from explosion to rb intersect any objects excluding our exceptions below
                if (Physics.Raycast(explosionPos, dir, out hit, dist, playerLayer, QueryTriggerInteraction.Ignore))
                {
                    if (hit.collider != null)
                    {
                        // Debug.Log("GameManager.Explode Hit...");
                        // Debug.Log("GameManager.Explode hit ==> " + hit);
                        // Debug.Log("hit.rigidbody ==> "+hit.rigidbody);
                        // Debug.Log("GameManager.Explode hit.transform.name ==> " + hit.transform.name);
                        // Debug.Log("rb ==> "+rb);
                        // Debug.Log("rb.transform ==> "+rb.transform);
                        // Debug.Log("_player.transform ==> "+_player.transform);

                        exposed =
                        hit.rigidbody == rb // collider is in rb's gameObject
                        || hit.transform.IsChildOf(rb.transform)
                        // || hit.transform == _player.transform
                        // || hit.transform.IsChildOf(_player.transform)
                        || (hit.rigidbody && hit.rigidbody.isKinematic)
                        // || hit.collider.isTrigger
                        || hit.transform.name.Contains("Ground");
                    }
                }
                if (exposed)
                {
                    // Debug.Log("GM Explode exposed = true");
                    IDamageable damageable = rb.GetComponent<IDamageable>();
                    if (damageable != null)
                    {
                        // Apply Damage 

                        float damagef = explosionForce;
                        damagef *= (radius - dist) / radius;
                        int damage = 1 + (int)Mathf.Ceil(damagef);

                        // Debug.Log("GameManager Explode hit [" + rb.name + "], damage to apply ==> " + damage);
                        // GameObject[] debris = 
                        // if (damage > 0)

                        damageable.TakeDamage(damage);
                        // Debug.Log("debris.length ==> "+debris.Length);
                    }
                    // else if (rb.name == "Head" || rb.name == "Torso" || rb.name == "Legs")
                    // {
                    //     List<GameObject> pieces = rb.GetComponent<SubdivideObjectScript>().SubdivideMe();
                    //     // Debug.Log("pieces.Length: "+pieces.Length);
                    //     // for (int p = 0; p < pieces.Count; p++)
                    //     // {

                    //     //     GameObject piece = pieces[p];
                    //     //     // Debug.Log("piece["+p+"]: "+piece);
                    //     //     piece.GetComponent<Rigidbody>().AddExplosionForce(explosionForce, explosionPos, radius, upwardsModifier);
                    //     // }
                    // }
                    else if (rb.gameObject == _player)
                    {
                        // This damage calculation is redundant with Mummy damage calculation above ^
                        // Player needs IDamageable interface

                        float damagef = explosionForce / 10f;
                        damagef *= (radius - dist) / radius;
                        int damage = 1 + (int)Mathf.Ceil(damagef);
                        // Debug.Log("GM.Explode, damage to Player:: " + damage);
                        if (damageable != null)
                        {
                            damageable.TakeDamage(damage);
                        }
                        Debug.Log("GM.Explode hit Player");
                    }
                    // else
                    // {
                    //     rb.AddExplosionForce(explosionForce, explosionPos, radius, upwardsModifier);
                    //     // Debug.Log("GM.Explode found no damageable");

                    // }
                    // }
                    // else
                    // {
                    // Not exposed
                    // Debug.Log("Collider "+rb.name+" Blocked By ==> "+hit.rigidbody.name);
                }
            }
            // else
            // {
            //     Debug.Log("GM.Explode coll[" + i + "] has no RB");
            // }
        }

        // Apply explosion force to nearby objects, and their rigidbody children

        Collider[] colliders1 = Physics.OverlapSphere(explosionPos, radius, ~0); //~0
        // Debug.Log("GM.Explode #colliders1[] :: " + colliders1.Length);
        for (int i = 0; i < colliders1.Length; i++)
        {
            Collider collider = colliders1[i];
            Rigidbody[] RBs = collider.GetComponentsInChildren<Rigidbody>();
            // Debug.Log("GM.Explode, collider[ " + collider.name + " ], #RBs :: " + RBs.Length);

            // If no child rbs, look for rigidbody in top parent
            if (RBs.Length == 0)
            {
                // Debug.Log("GameManager - NO RBs");

                // Collider has no Rigidbody
                // Look for Rigidbody in root parent
                //Transform topParent = collider.transform.root;
                Rigidbody topRB = collider.GetComponentInParent<Rigidbody>();// topParent.GetComponent<Rigidbody>();

                if (topRB != null)
                {
                    // Debug.Log("topRB:: " + topRB.name);
                    topRB.AddExplosionForce(explosionForce * 10, explosionPos, radius, upwardsModifier);
                }
            }
            else
            {
                foreach (Rigidbody rb in RBs)
                {

                    if (rb != null) // && !rb.CompareTag("Player") )
                    {
                        // Does the ray intersect any objects excluding the rb
                        // bool exposed = true;
                        // RaycastHit hit;
                        // Vector3 dir = (explosionPos - rb.position).normalized;
                        // float dist = Vector3.Distance(rb.position, explosionPos);
                        // // Does the ray (from rb to explosion center) intersect any objects excluding the player layer
                        // (Physics.Raycast(rb.transform.position, dir, out hit, dist))
                        // {
                        //     if(hit.rigidbody != null){
                        //         exposed =  rb == hit.rigidbody 
                        //         || hit.transform.IsChildOf(rb.transform) 
                        //         || hit.transform == _player.transform 
                        //         || hit.transform.IsChildOf(_player.transform) 
                        //         || hit.rigidbody.isKinematic 
                        //         || hit.transform.name.Contains("Ground"); 
                        //     }
                        // }

                        // if( exposed ) {
                        //     rb.AddExplosionForce(explosionForce, explosionPos, radius, upwardsModifier);
                        // }
                        // Add force to all nearby rigidbodies, regardless of whether exposed
                        // Debug.Log("GM.Explode collider[ " + collider.name + " ], rb[ " + rb.name + " ]");
                        rb.AddExplosionForce(explosionForce * 10, explosionPos, radius, upwardsModifier);
                    }
                }
            }
        }


        // show explosion
        if (explosionGO != null)
        {
            GameObject expl = (GameObject)Instantiate(explosionGO, explosionPos, Quaternion.identity);

            Destroy(expl, 3); // delete the explosion after 3 seconds
            // Debug.Log("expl: " + expl);
        }

        CauseCommotion(explosionPos, radius * 2f, explosionForce * 0.3f);

    }

    public void CauseCommotion(Vector3 position, float radius, float duration)
    {
        //return; // <-- CauseCommotion Stalls game.  Find solution.
        Collider[] colliders = Physics.OverlapSphere(position, radius, _enemiesMask);
        foreach (Collider coll in colliders)
        {
            if (coll.name.Contains("Mummy") && !coll.name.Contains("Separated"))
            {
                coll.gameObject.GetComponent<EnemyAIScript>().FacePosition(position);
            }
        }


        // Debug.Log("GM.CauseCommotion.Duration:: " + duration);
        // GameObject commotion = Instantiate(commotionPrefab, position, Quaternion.identity);
        // commotion.GetComponent<CommotionScript>().Duration = duration;
        // commotion.GetComponent<CommotionScript>().Radius = radius;
    }

    void SetPaused(bool value)
    {
        _gameIsPaused = value;
        if (value)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1;
        }
        AudioListener.pause = value;
        pausedScreen.gameObject.SetActive(value);
    }

    public GameObject FindChildByName(GameObject parent, string name)
    {
        Transform[] trs = parent.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in trs)
        {
            if (t.name == name)
            {
                return t.gameObject;
            }
        }
        return null;
    }

    /*
    GETTERS / SETTERS
    */
    public bool GamePaused
    {
        get { return _gameIsPaused; }
        set
        {
            // if (value is bool)
            // {
            SetPaused(value);
            // }
        }
    }
    public int DifficultyLevel
    {
        get => _difficultyLevel;
    }

    public int Score
    {
        get { return _score; }
        set
        {
            _score = value;
            string scoreString = _score.ToString();
            scoreText.text = scoreString;
        }
    }
}

