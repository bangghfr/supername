using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YG;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource ambientSource;
    public AudioSource musicSource;

    private void Awake()
    {
        // Реализация Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Инициализация источников, если не назначены
        if (ambientSource == null)
        {
            ambientSource = gameObject.AddComponent<AudioSource>();
            ambientSource.loop = true; // Предпочтительно для амбиентных звуков
        }

        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true; // Музыка обычно тоже зациклена
        }
    }

    // Управление звука Ambient
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

    // Управление музыкой
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
}
