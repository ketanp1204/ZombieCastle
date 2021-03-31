using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SetGlobalVolume : MonoBehaviour
{
    private void Start()
    {
        AudioListener.volume = 1f;
    }

    public void SetGlobalAudioVolume(float volume)
    {
        AudioListener.volume = volume;
    }
}
