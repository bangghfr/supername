using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YG;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    public enum SoundType
    {
        Ambient,
        Music,
        Jump,
        Attack
    }

    [Header("Audio Sources")]
    public AudioSource ambientSource;
    public AudioSource musicSource;
    public AudioSource effectSource;

    [Header("Audio Clips")]
    public AudioClip jumpClip;
    public AudioClip attackClip;
    public AudioClip MusicClip;
    public AudioClip AmbientClip;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (ambientSource == null)
        {
            ambientSource = gameObject.AddComponent<AudioSource>();
            ambientSource.loop = true;
        }

        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
        }

        if (effectSource == null)
        {
            effectSource = gameObject.AddComponent<AudioSource>();
            effectSource.loop = false;
        }
    }

    public void PlaySound(SoundType type)
    {
        switch (type)
        {
            case SoundType.Ambient:
                // Можно вызвать PlayAmbient с нужным клипом, если есть
                break;
            case SoundType.Music:
                // Можно вызвать PlayMusic с нужным клипом
                break;
            case SoundType.Jump:
                if (jumpClip != null)
                    PlayEffect(jumpClip);
                break;
            case SoundType.Attack:
                if (attackClip != null)
                    PlayEffect(attackClip);
                break;
        }
    }

    public void PlayAmbient(AudioClip clip)
    {
        if (ambientSource.isPlaying)
            ambientSource.Stop();

        ambientSource.clip = clip;
        ambientSource.Play();
    }

    public void StopAmbient()
    {
        ambientSource.Stop();
    }

    public void SetAmbientVolume(float volume)
    {
        ambientSource.volume = Mathf.Clamp01(volume);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (musicSource.isPlaying)
            musicSource.Stop();

        musicSource.clip = clip;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = Mathf.Clamp01(volume);
    }

    public void PlayEffect(AudioClip clip)
    {
        effectSource.PlayOneShot(clip);
    }

    public void SetEffectVolume(float volume)
    {
        effectSource.volume = Mathf.Clamp01(volume);
    }
}