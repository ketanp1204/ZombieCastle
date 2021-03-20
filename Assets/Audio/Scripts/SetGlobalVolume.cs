using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SetGlobalVolume : MonoBehaviour
{
    public void SetGlobalAudioVolume(float volume)
    {
        AudioListener.volume = volume;
    }
}
