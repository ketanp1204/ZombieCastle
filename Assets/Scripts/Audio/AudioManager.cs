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
        ZombieDeath,
        TitleScreenTrack,
        IntroSequenceBackground,
        IntroSequenceBlackout,
        IntroSequenceCharacterGrassWalk,
        TextAutoTypingSound
    }

    // private static Dictionary<Sound, float> soundTimerDictionary;
    public static Dictionary<Sound, GameObject> playSoundOnceGameObjects;
    public static Dictionary<Sound, GameObject> loopingSoundGameObjects;
    public static Dictionary<Sound, GameObject> playOneShotGameObjects;

    public static AudioClip currentAudioClip;

    public static void Initialize()
    {
        // soundTimerDictionary = new Dictionary<Sound, float>();
        playSoundOnceGameObjects = new Dictionary<Sound, GameObject>();
        loopingSoundGameObjects = new Dictionary<Sound, GameObject>();
        playOneShotGameObjects = new Dictionary<Sound, GameObject>();
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

    public static void PlayOneShotSound(Sound sound)
    {
        if (playOneShotGameObjects.ContainsKey(sound))
        {
            AudioSource audioSource = playOneShotGameObjects[sound].GetComponent<AudioSource>();
            audioSource.PlayOneShot(audioSource.clip);
        }
        else
        {
            GameObject playOneShotGO = new GameObject("OneShotSound");
            playOneShotGO.AddComponent<DontDestroyGameObjectOnLoad>();
            AudioSource audioSource = playOneShotGO.AddComponent<AudioSource>();

            // Add gameobject to dictionary
            playOneShotGameObjects.Add(sound, playOneShotGO);

            // Get sound data scriptable object
            SoundData soundData = GetSoundData(sound);

            // Set sound properties in AudioSource
            audioSource.clip = soundData.audioClip;
            audioSource.loop = soundData.loopSound;
            audioSource.volume = soundData.volume;
            audioSource.PlayOneShot(soundData.audioClip);
        }
    }

    public static void PlaySoundOnce(Sound sound)
    {
        if (CanPlaySound(sound))
        {
            if (playSoundOnceGameObjects.ContainsKey(sound))
            {
                playSoundOnceGameObjects[sound].GetComponent<AudioSource>().Play();
            }
            else
            {
                GameObject playSoundOnceGO = new GameObject("PlayOnceSound");
                playSoundOnceGO.AddComponent<DontDestroyGameObjectOnLoad>();
                AudioSource audioSource = playSoundOnceGO.AddComponent<AudioSource>();

                // Add gameobject to dictionary
                playSoundOnceGameObjects.Add(sound, playSoundOnceGO);

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

    public static void PlaySoundLooping(Sound sound)
    {
        if (CanPlaySound(sound))
        {
            if (loopingSoundGameObjects.ContainsKey(sound))
            {
                // Get AudioSource
                AudioSource audioSource = loopingSoundGameObjects[sound].GetComponent<AudioSource>();

                // If not already playing, play it
                if (!audioSource.isPlaying)
                {
                    audioSource.Play();
                }
            }
            else
            {
                // Create new GameObject for the sound
                GameObject loopingSoundGO = new GameObject("LoopingSound");
                loopingSoundGO.AddComponent<DontDestroyGameObjectOnLoad>();

                // Create AudioSource
                AudioSource audioSource = loopingSoundGO.AddComponent<AudioSource>();

                // Add gameobject to dictionary
                loopingSoundGameObjects.Add(sound, loopingSoundGO);

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

    private static bool IsSoundPlaying(Sound sound)
    {
        if (playSoundOnceGameObjects.ContainsKey(sound))
        {
            if (playSoundOnceGameObjects[sound].GetComponent<AudioSource>().isPlaying)
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
                /*
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
                */
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
