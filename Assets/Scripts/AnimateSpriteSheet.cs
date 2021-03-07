using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class AnimateSpriteSheet : MonoBehaviour
{
    public int Columns = 5;
    public int Rows = 5;
    public float FramesPerSecond = 10f;
    public bool RunOnce = true;
    public bool RandomStart = true;

    private Renderer _renderer;

    public float RunTimeInSeconds
    {
        get
        {
            return ((1f / FramesPerSecond) * (Columns * Rows));
        }
    }

    private Material materialCopy = null;
    void Awake()
    {
        // Debug.Log("AnimateSS Awake()");
        _renderer = GetComponent<Renderer>();
    }

    void Start()
    {
        // Debug.Log("AnimateSS Start()");

        // Copy its material to itself in order to create an instance not connected to any other
        materialCopy = new Material(_renderer.sharedMaterial);
        _renderer.sharedMaterial = materialCopy;

        Vector2 size = new Vector2(1f / Columns, 1f / Rows);
        _renderer.sharedMaterial.SetTextureScale("_MainTex", size);

    }

    void OnEnable()
    {
        // Debug.Log("AnimateSS OnEnable()");

        StartCoroutine(UpdateTiling());
    }

    private IEnumerator UpdateTiling()
    {
        if (RandomStart)
        {
            float totalDuration = (Rows * Columns) / FramesPerSecond;
            float waitSecs = Random.Range(0f, totalDuration);
            yield return new WaitForSeconds(waitSecs);
        }

        float x = 0f;
        float y = 0f;
        Vector2 offset = Vector2.zero;

        while (true)
        {
            for (int i = Rows - 1; i >= 0; i--) // y
            {
                y = (float)i / Rows;

                for (int j = 0; j <= Columns - 1; j++) // x
                {
                    x = (float)j / Columns;

                    offset.Set(x, y);

                    _renderer.sharedMaterial.SetTextureOffset("_MainTex", offset);
                    yield return new WaitForSeconds(1f / FramesPerSecond);
                }
            }

            if (RunOnce)
            {
                yield break;
            }
        }
    }
}