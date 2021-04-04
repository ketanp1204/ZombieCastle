using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetGlobalVolume : MonoBehaviour
{
    public Slider volumeSlider;

    private void Start()
    {
        AudioListener.volume = GameData.audioVolume;
        volumeSlider.value = GameData.audioVolume;
    }

    public void SetGlobalAudioVolume(float volume)
    {
        GameData.audioVolume = volume;
        AudioListener.volume = GameData.audioVolume;
    }
}
