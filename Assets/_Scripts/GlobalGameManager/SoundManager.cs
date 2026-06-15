using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public interface ISoundManager
{
    void PlayBGM(AudioClip clip, float fadeTime = 1f);
    void PlaySFX(AudioClip clip);
    void PlaySFX(AudioClip clip, Vector3 position);
}
public class SoundManager : MonoBehaviour, ISoundManager
{
    static ISoundManager instance;
    public static ISoundManager Instance { get => instance; }
    [SerializeField] private AudioClip initialBGM;
    [SerializeField] private AudioMixerGroup _bgmGroup;
    [SerializeField] private AudioMixerGroup _sfxGroup;
    int poolSize = 5;
    private Queue<AudioSource> sfx_pool = new();

    private AudioSource bgm_sourceA;
    private AudioSource bgm_sourceB;
    private AudioSource bgm_current;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        // CrossFade용 AudioSource 2개
        bgm_sourceA = CreateSource("BGM_A", _bgmGroup);
        bgm_sourceB = CreateSource("BGM_B", _bgmGroup);
        bgm_current = bgm_sourceA;


        sfx_pool = new Queue<AudioSource>();
        for (int i = 0; i < poolSize; i++)
        {
            var src = CreateSource($"SFX_Group_{i}", _sfxGroup, false, false);
            sfx_pool.Enqueue(src);
        }

        PlayBGM(initialBGM);
    }
    public void PlayBGM(AudioClip clip, float fadeTime = 1f)
    {
        StartCoroutine(CrossFade(clip, fadeTime));
    }
    public void PlaySFX(AudioClip clip)
    {
        var src = GetAvailableSource(sfx_pool);
        src.clip = clip;
        src.Play();
    }
    public void PlaySFX(AudioClip clip, Vector3 position)
    {
        var src = GetAvailableSource(sfx_pool);
        src.transform.position = position;
        src.spatialBlend = 1f;
        src.clip = clip;
        src.Play();
    }

    private AudioSource CreateSource(string goName, AudioMixerGroup group, bool loop = true, bool playOnAwake = true)
    {
        var go = new GameObject(goName);
        go.transform.SetParent(transform);
        var src = go.AddComponent<AudioSource>();
        src.outputAudioMixerGroup = group;
        src.loop = loop;
        src.playOnAwake = playOnAwake;
        return src;
    }

    private IEnumerator CrossFade(AudioClip clip, float fadeTime)
    {
        var next = (bgm_current == bgm_sourceA) ? bgm_sourceB : bgm_sourceA;
        next.clip = clip;
        next.volume = 0f;
        next.Play();

        float elapsed = 0f;
        float startVol = bgm_current.volume;

        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeTime;
            bgm_current.volume = Mathf.Lerp(startVol, 0f, t);
            next.volume = Mathf.Lerp(0f, 1f, t);
            yield return null;
        }

        bgm_current.Stop();
        bgm_current = next;
    }

    private AudioSource GetAvailableSource(Queue<AudioSource> pool)
    {
        int count = pool.Count;
        for (int i = 0; i < count; i++)
        {
            var src = pool.Dequeue();
            pool.Enqueue(src);
            if (!src.isPlaying) return src;
        }
        return pool.Peek();
    }
}