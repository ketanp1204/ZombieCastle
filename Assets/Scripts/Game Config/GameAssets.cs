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

    [Header("Treasure Box Sprites")]
    public Sprite treasureBoxOpenSprite;
    public Sprite treasureBoxClosedSprite;

    [Header("ParticleSystem Prefabs")]
    public GameObject bloodParticles;

    [Header("Maze Puzzle Sprites")]
    public Sprite switchOffSprite;
    public Sprite switchOnSprite;

    [Header("Note Box Sprites")]
    public Sprite noteBoxLarge;
    public Sprite noteBoxSmall;

    [Header("Castle Lobby first start dialogue")]
    [TextArea(4, 10)]
    public string[] gameStartDialogue;

    [Header("Player start inventory")]
    public InventoryObject playerStartInventory;

    [Header("Room 1")]
    public PC_Then_Inventory_Object r1_barrel_oil_collectable;
    
}
