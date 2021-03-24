﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectProperties : MonoBehaviour
{
    private enum PC_Then_Inventory_Objects
    {
        Lobby_Key,
        Lobby_Torch
    }

    public ItemObject objectData;
    public GameObject imageDisplayGO;
    private SpriteGlow.SpriteGlowEffect spriteGlowEffectComponent;
    public AudioManager.Sound descBoxItemReceivedSound;

    // Start is called before the first frame update
    void Start()
    {
        spriteGlowEffectComponent = GetComponent<SpriteGlow.SpriteGlowEffect>();
        spriteGlowEffectComponent.enabled = false;
    }

    public void UpdateGameDataForPCThenInventoryObject(PC_Then_Inventory_Object item)
    {
        string itemName = item.sceneName + "_" + item.itemName;

        if (itemName == PC_Then_Inventory_Objects.Lobby_Torch.ToString())
        {
            GameData.lobby_torchCollected = true;
        }
        else if (itemName == PC_Then_Inventory_Objects.Lobby_Key.ToString())
        {
            GameData.lobby_keyCollected = true;
        }
    }
}
