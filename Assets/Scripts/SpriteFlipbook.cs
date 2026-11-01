using System.Collections.Generic;
using UnityEngine;

// Plays a horizontal sprite sheet on a SpriteRenderer (used for the spinning coin).
// Put the sheet PNG in Assets/Resources/ and type its name below.
[RequireComponent(typeof(SpriteRenderer))]
public class SpriteFlipbook : MonoBehaviour
{
    public string resourceName = "CoinSheet";
    public int frameSize = 16;
    public float pixelsPerUnit = 16f;
    public float fps = 12f;

    static readonly Dictionary<string, Sprite[]> cache = new Dictionary<string, Sprite[]>();

    SpriteRenderer sr;
    Sprite[] frames;
    float offset;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        offset = Random.value * 10f; // so coins don't all spin in sync

        if (!cache.TryGetValue(resourceName, out frames) || frames == null || frames.Length == 0 || frames[0] == null)
        {
            var tex = Resources.Load<Texture2D>(resourceName);
            if (tex == null)
            {
                Debug.LogWarning("SpriteFlipbook: put '" + resourceName + ".png' in Assets/Resources/");
                enabled = false;
                return;
            }
            tex.filterMode = FilterMode.Point;
            int n = tex.width / frameSize;
            frames = new Sprite[n];
            for (int i = 0; i < n; i++)
                frames[i] = Sprite.Create(tex, new Rect(i * frameSize, 0, frameSize, tex.height),
                                          new Vector2(0.5f, 0.5f), pixelsPerUnit);
            cache[resourceName] = frames;
        }
    }

    void Update()
    {
        int i = (int)((Time.time + offset) * fps) % frames.Length;
        sr.sprite = frames[i];
    }
}
