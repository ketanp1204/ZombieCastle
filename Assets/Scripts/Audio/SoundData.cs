using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSoundData", menuName = "SoundData")]
public class SoundData : ScriptableObject
{
    public AudioClip audioClip;
    public bool loopSound;
    public float volume;
}
