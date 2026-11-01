using System.Collections.Generic;
using UnityEngine;

// Code-only pixel particles + camera shake. No assets or setup needed:
// the first call creates a hidden "PixelFX" object automatically.
public class PixelFX : MonoBehaviour
{
    static PixelFX instance;
    static Sprite square;

    class P
    {
        public Transform t;
        public SpriteRenderer sr;
        public Vector2 pos, vel;
        public float life, maxLife;
        public Color color;
    }

    readonly List<P> particles = new List<P>();
    Camera cam;
    Vector3 camBase;
    float shakeTime, shakeDur, shakeMag;

    static readonly Color[] Gold = { new Color(1f, .82f, .16f), new Color(1f, .95f, .6f), new Color(.84f, .59f, .08f), Color.white };
    static readonly Color[] Fire = { new Color(1f, .55f, .12f), new Color(1f, .85f, .25f), new Color(.9f, .2f, .15f), new Color(.2f, .2f, .28f), Color.white };
    static readonly Color[] Dust = { new Color(.55f, .55f, .6f), new Color(.4f, .4f, .45f), new Color(.7f, .7f, .75f) };

    // ---------- public helpers ----------
    public static void CoinBurst(Vector2 pos) { Get().Burst(pos, Gold, 14, 5f, 0.55f); }

    public static void BombBurst(Vector2 pos)
    {
        Get().Burst(pos, Fire, 32, 8f, 0.75f);
        Shake(0.3f, 0.3f);
    }

    public static void GroundPuff(Vector2 pos) { Get().Burst(pos, Dust, 7, 2.5f, 0.4f); }

    public static void Shake(float duration = 0.25f, float magnitude = 0.25f)
    {
        var fx = Get();
        fx.shakeDur = duration;
        fx.shakeTime = duration;
        fx.shakeMag = magnitude;
    }

    // ---------- internals ----------
    static PixelFX Get()
    {
        if (instance == null)
            instance = new GameObject("PixelFX").AddComponent<PixelFX>();
        return instance;
    }

    void Awake()
    {
        cam = Camera.main;
        if (cam) camBase = cam.transform.position;

        if (square == null)
        {
            var tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            tex.SetPixel(0, 0, Color.white);
            tex.Apply();
            tex.filterMode = FilterMode.Point;
            square = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 16f);
        }
    }

    void Burst(Vector2 pos, Color[] colors, int count, float speed, float life)
    {
        for (int i = 0; i < count; i++)
        {
            var go = new GameObject("p");
            go.transform.SetParent(transform, false);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = square;
            sr.sortingOrder = 50;
            int size = Random.Range(2, 4); // 2-3 "pixels" wide
            go.transform.localScale = new Vector3(size, size, 1f);

            float a = Random.value * Mathf.PI * 2f;
            float s = Random.Range(speed * 0.35f, speed);
            var p = new P
            {
                t = go.transform,
                sr = sr,
                pos = pos,
                vel = new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * s + Vector2.up * speed * 0.3f,
                maxLife = life * Random.Range(0.7f, 1.1f),
                color = colors[Random.Range(0, colors.Length)]
            };
            p.life = p.maxLife;
            sr.color = p.color;
            go.transform.position = Snap(pos);
            particles.Add(p);
        }
    }

    static Vector3 Snap(Vector2 v)
    {
        return new Vector3(Mathf.Round(v.x * 16f) / 16f, Mathf.Round(v.y * 16f) / 16f, 0f);
    }

    void Update()
    {
        float dt = Time.unscaledDeltaTime; // keeps playing even when the game freezes on game over
        for (int i = particles.Count - 1; i >= 0; i--)
        {
            var p = particles[i];
            p.life -= dt;
            if (p.life <= 0f)
            {
                Destroy(p.t.gameObject);
                particles.RemoveAt(i);
                continue;
            }
            p.vel.y -= 14f * dt;
            p.pos += p.vel * dt;
            p.t.position = Snap(p.pos);

            float k = p.life / p.maxLife;
            var c = p.color;
            c.a = k < 0.3f ? (Mathf.FloorToInt(k * 30f) % 2 == 0 ? 1f : 0.35f) : 1f; // blink out
            p.sr.color = c;
        }
    }

    void LateUpdate()
    {
        if (cam == null) return;
        if (shakeTime > 0f)
        {
            shakeTime -= Time.unscaledDeltaTime;
            float m = shakeMag * Mathf.Clamp01(shakeTime / shakeDur);
            Vector2 o = Random.insideUnitCircle * m;
            cam.transform.position = camBase + Snap(o);
        }
        else
        {
            cam.transform.position = camBase;
        }
    }
}
