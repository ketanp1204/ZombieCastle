using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AudioManager
{
    public enum Sound
    {
        BackgroundTrack,
        PlayerAxeHit,
        ZombieEating,
        ZombieGroan
    }

    private static Dictionary<Sound, float> soundTimerDictionary;
    private static GameObject oneShotGameObject;
    private static AudioSource oneShotAudioSource;

    public static void Initialize()
    {
        soundTimerDictionary = new Dictionary<Sound, float>();
        soundTimerDictionary[Sound.PlayerAxeHit] = 0f;
    }

    public static void PlaySound(Sound sound, Vector3 position)
    {
        if (CanPlaySound(sound))
        {
            GameObject soundGameObject = new GameObject("Sound");
            soundGameObject.transform.position = position;
            AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
            audioSource.clip = GetAudioClip(sound);
            audioSource.Play();

            Object.Destroy(soundGameObject, audioSource.clip.length);
        }
    }

    public static void PlaySound(Sound sound)
    {
        if (CanPlaySound(sound) && !IsSoundPlaying(sound))
        {
            if (oneShotGameObject == null)
            {
                oneShotGameObject = new GameObject("One Shot Sound");
                oneShotGameObject.AddComponent<DontDestroyGameObjectOnLoad>();
                oneShotAudioSource = oneShotGameObject.AddComponent<AudioSource>();
            }
            oneShotAudioSource.PlayOneShot(GetAudioClip(sound));
        }
    }

    public static void PlaySoundLooping(Sound sound)
    {
        if (CanPlaySound(sound) && !IsSoundPlaying(sound))
        {
            if (oneShotGameObject == null)
            {
                oneShotGameObject = new GameObject("One Shot Sound");
                oneShotGameObject.AddComponent<DontDestroyGameObjectOnLoad>();
                oneShotAudioSource = oneShotGameObject.AddComponent<AudioSource>();
            }
            oneShotAudioSource.volume = 0.5f;   // FOR TESTING. REMOVE LATER
            oneShotAudioSource.loop = true;
            oneShotAudioSource.clip = GetAudioClip(sound);
            oneShotAudioSource.Play();
        }
    }

    private static bool IsSoundPlaying(Sound sound)
    {
        if (oneShotGameObject != null)
        {
            if (oneShotAudioSource.isPlaying)
            {
                if (oneShotAudioSource.clip == GetAudioClip(sound))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private static bool CanPlaySound(Sound sound)
    {
        switch (sound)
        {
            default:
                return true;
            case Sound.PlayerAxeHit:
                if (soundTimerDictionary.ContainsKey(sound))
                {
                    float lastTimePlayed = soundTimerDictionary[sound];
                    float playerAxeHitTimerMax = 0.4f;
                    if (lastTimePlayed + playerAxeHitTimerMax < Time.time)
                    {
                        soundTimerDictionary[sound] = Time.time;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
        }
    }

    private static AudioClip GetAudioClip(Sound sound)
    {
        foreach (GameAssets.SoundAudioClip soundAudioClip in GameAssets.instance.soundAudioClipArray)
        {
            if (soundAudioClip.sound == sound)
            {
                return soundAudioClip.audioClip;
            }
        }
        Debug.LogError("Sound " + sound + " not found!");
        return null;
    }
}
