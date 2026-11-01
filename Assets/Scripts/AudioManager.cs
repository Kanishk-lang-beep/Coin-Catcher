using System.Collections.Generic;
using UnityEngine;

// Loads sounds from Assets/Resources by name and plays them.
// Creates itself automatically on first use - no scene setup needed
// (just make sure your Main Camera has its AudioListener, which it does by default).
public class AudioManager : MonoBehaviour
{
    static AudioManager instance;

    AudioSource sfx, music;
    readonly Dictionary<string, AudioClip> clips = new Dictionary<string, AudioClip>();

    public float sfxVolume = 1f;
    public float musicVolume = 0.35f;

    static AudioManager Get()
    {
        if (instance == null)
            instance = new GameObject("AudioManager").AddComponent<AudioManager>();
        return instance;
    }

    void Awake()
    {
        sfx = gameObject.AddComponent<AudioSource>();
        sfx.playOnAwake = false;

        music = gameObject.AddComponent<AudioSource>();
        music.playOnAwake = false;
        music.loop = true;
    }

    AudioClip Load(string name)
    {
        AudioClip c;
        if (!clips.TryGetValue(name, out c))
        {
            c = Resources.Load<AudioClip>(name);
            if (c == null) Debug.LogWarning("AudioManager: '" + name + "' not found in Assets/Resources/");
            clips[name] = c;
        }
        return c;
    }

    // ---------- public API ----------
    public static void Play(string name, float volume = 1f, float pitchVariation = 0.06f)
    {
        var a = Get();
        var c = a.Load(name);
        if (c == null) return;
        a.sfx.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);
        a.sfx.PlayOneShot(c, volume * a.sfxVolume);
    }

    public static void PlayMusic()
    {
        var a = Get();
        var c = a.Load("music");
        if (c == null) return;
        a.music.clip = c;
        a.music.volume = a.musicVolume;
        if (!a.music.isPlaying) a.music.Play();
    }

    public static void StopMusic()
    {
        if (instance != null) instance.music.Stop();
    }
}
