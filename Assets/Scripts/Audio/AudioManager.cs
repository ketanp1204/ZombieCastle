using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AudioManager
{
    public enum Sound
    {
        BackgroundTrack,
        DoorOpen,
        PlayerFootStep,
        PlayerAxeAttack,
        PlayerKnifeAttack,
        PlayerSwordAttack,
        PlayerGettingHit,
        Zombie1GettingHit,
        Zombie1Roar,
        Zombie2GettingHit,
        Zombie2Roar,
        Zombie3GettingHit,
        Zombie3Roar,
        Zombie4GettingHit,
        Zombie4Roar,
        ZombieDeath
    }

    private static Dictionary<Sound, float> soundTimerDictionary;
    private static GameObject oneShotGameObject;
    private static AudioSource oneShotAudioSource;
    private static Dictionary<Sound, GameObject> oneShotGameObjects;

    public static void Initialize()
    {
        soundTimerDictionary = new Dictionary<Sound, float>();
        soundTimerDictionary[Sound.PlayerAxeAttack] = 0f;
        soundTimerDictionary[Sound.PlayerFootStep] = 0f;

        oneShotGameObjects = new Dictionary<Sound, GameObject>();
    }

    public static void PlaySoundAtPosition(Sound sound, Vector3 position)
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

    public static void PlaySoundOnce(Sound sound)
    {
        if (CanPlaySound(sound) && !IsSoundPlaying(sound))
        {
            if (oneShotGameObjects.ContainsKey(sound))
            {
                oneShotGameObjects[sound].GetComponent<AudioSource>().Play();
            }
            else
            {
                GameObject oneShotGO = new GameObject("OneShotSound");
                oneShotGO.AddComponent<DontDestroyGameObjectOnLoad>();
                AudioSource audioSource = oneShotGO.AddComponent<AudioSource>();

                // Add gameobject to dictionary
                oneShotGameObjects.Add(sound, oneShotGO);

                // Get sound data scriptable object
                SoundData soundData = GetSoundData(sound);

                // Set sound properties in AudioSource
                audioSource.clip = soundData.audioClip;
                audioSource.loop = soundData.loopSound;
                audioSource.volume = soundData.volume;
                audioSource.Play();
            }
        }
    }

    public static void PlayNewSoundLooping(Sound sound)
    {
        if (CanPlaySound(sound))
        {
            GameObject soundGameObject = new GameObject("Sound");
            soundGameObject.AddComponent<DontDestroyGameObjectOnLoad>();

            AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();

            // Get sound data scriptable object
            SoundData soundData = GetSoundData(sound);

            // Set sound properties in AudioSource
            audioSource.clip = soundData.audioClip;
            audioSource.loop = soundData.loopSound;
            audioSource.volume = soundData.volume;

            audioSource.Play();
        }
    }

    private static bool IsSoundPlaying(Sound sound)
    {
        if (oneShotGameObjects.ContainsKey(sound))
        {
            if (oneShotGameObjects[sound].GetComponent<AudioSource>().isPlaying)
            {
                return true;
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

            case Sound.PlayerAxeAttack:
                if (soundTimerDictionary.ContainsKey(sound))
                {
                    float lastTimePlayed = soundTimerDictionary[sound];
                    float playerAxeHitTimerMax = 0.1f;
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
                return soundAudioClip.soundData.audioClip;
            }
        }
        Debug.LogError("Sound " + sound + " not found!");
        return null;
    }

    private static SoundData GetSoundData(Sound sound)
    {
        foreach (GameAssets.SoundAudioClip soundAudioClip in GameAssets.instance.soundAudioClipArray)
        {
            if (soundAudioClip.sound == sound)
            {
                return soundAudioClip.soundData;
            }
        }
        Debug.LogError("Sound " + sound + " not found!");
        return null;
    }
}
