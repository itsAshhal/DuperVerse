using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] bool isSoundMuted = false;

    [SerializeField] public AudioSource musicSource;
    [SerializeField] public AudioSource buttonSource;
    [SerializeField] AudioSource[] soundEffectSources;
    [SerializeField] AudioSource loopedSoundEffectSource;
    [SerializeField] AudioSource soundEffectOneShotSource;
    [SerializeField] AudioSource soundEffectOneShotSource1;

    [SerializeField] List<AudioClip> musicClips;
    [SerializeField] List<AudioClip> buttonClips;

    [SerializeField] List<AudioClip> soundEffectClips;

    [Header("Soldier Audio Clips")]
    public List<AudioClip> AudioClips;
    public AudioClip AudioDummy;
   
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void PlayMusic(int index)
    {
        if (isSoundMuted) return;
        if (musicSource.isPlaying)
            musicSource.Stop();

        musicSource.clip = musicClips[index];
        musicSource.Play();
    }

    public void PlaySoundEffect(AudioClip clip, int sourceIndex)
    {
        if (isSoundMuted) return;
        if (clip == null)
        {
            Debug.LogError("No clip found, please check variable and pass clip accordingly.");
            return;
        }

        if (sourceIndex >= soundEffectSources.Length || sourceIndex < 0)
        {
            Debug.LogError("Source not found. please pass a valid index");
            return;
        }

        soundEffectSources[sourceIndex].Stop();
        soundEffectSources[sourceIndex].clip = null;

        soundEffectSources[sourceIndex].clip = clip;
        soundEffectSources[sourceIndex].Play();
    }

    public void PlaySoundEffectOneShot(AudioClip clip)
    {
        if (isSoundMuted) return;
        if (clip == null)
        {
            Debug.LogError("No clip found, please check variable and pass clip accordingly.");
            return;
        }

        soundEffectOneShotSource.PlayOneShot(clip);
    }

    public void PlaySoundEffectOneShot1(AudioClip clip)
    {
        if (isSoundMuted) return;
        if (clip == null)
        {
            Debug.LogError("No clip found, please check variable and pass clip accordingly.");
            return;
        }

        soundEffectOneShotSource1.PlayOneShot(clip);
    }

    public void PlayLoopedSoundEffect(AudioClip clip)
    {
        if (isSoundMuted) return;
        if (clip == null)
        {
            //Debug.LogError("No clip found, please check variable and pass clip accordingly.");
            return;
        }

        if (loopedSoundEffectSource.clip == clip) return;

        loopedSoundEffectSource.Stop();

        loopedSoundEffectSource.clip = clip;
        loopedSoundEffectSource.Play();
    }

    public void StopOneShotSource()
    {
        soundEffectOneShotSource.Stop();
    }

    public void StopSoundEffectSource(int sourceIndex)
    {
        soundEffectSources[sourceIndex].Stop();
        soundEffectSources[sourceIndex].clip = null;
    }

    public void StopLoopedSoundEffectSource()
    {
        loopedSoundEffectSource.Stop();
        loopedSoundEffectSource.clip = null;
    }
    public void PlayButtonSound(int index)
    {
        if (isSoundMuted) return;
        if (buttonSource.isPlaying)
            buttonSource.Stop();
        Debug.Log("Play Button Sound");
        buttonSource.clip = buttonClips[index];
        buttonSource.Play();
    }
}
