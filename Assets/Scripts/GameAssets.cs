using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{

    private static GameAssets _instance;

    public static GameAssets instance 
    { 
        get 
        { 
            if (_instance == null) 
                _instance = (Instantiate(Resources.Load("GameAssets")) as GameObject).GetComponent<GameAssets>();
            return _instance; 
        } 
    }

    [Header("Sounds")]
    public SoundAudioClip[] soundAudioClipArray;

    [System.Serializable]
    public class SoundAudioClip
    {
        public AudioManager.Sound sound;
        // public AudioClip audioClip;
        public SoundData soundData;
    }

    [Header("UI Objects")]
    public GameObject interactKeyPrefab;
    public GameObject objectNamePrefab;

    [Header("Sprites")]
    public Sprite treasureBoxOpenSprite;
    public Sprite treasureBoxClosedSprite;

    [Header("ParticleSystem Prefabs")]
    public GameObject bloodParticles;

    [Header("Other GameObjects")]
    public GameObject mazePuzzleColliderGO;
    
}
