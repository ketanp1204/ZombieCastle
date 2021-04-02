using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectProperties : MonoBehaviour
{
    private enum PC_Then_Inventory_Objects
    {
        Lobby_Key,
        Lobby_Torch,
        Room1_Barrel
    }

    private enum Desc_Box_Then_Dialogue_Objects
    {
        Room2_Drawer,
        Room5_Drawer
    }

    private enum PC_Then_Note_Objects
    {
        Lobby_Paper,
        Room3_Drawer
    }

    private SpriteGlow.SpriteGlowEffect glowEffect;

    public ItemObject objectData;
    public GameObject imageDisplayGO;
    public AudioManager.Sound descBoxItemReceivedSound;

    // Start is called before the first frame update
    void Start()
    {
        glowEffect = GetComponent<SpriteGlow.SpriteGlowEffect>();
        glowEffect.enabled = false;

        CheckIfAlreadyInteractedWithObject();
    }

    private void CheckIfAlreadyInteractedWithObject()
    {
        if (objectData.itemType == ItemType.PC_Then_Inventory)
        {
            PC_Then_Inventory_Object obj = (PC_Then_Inventory_Object)objectData;

            string itemName = obj.sceneName + "_" + obj.itemName;

            if (itemName == PC_Then_Inventory_Objects.Lobby_Key.ToString())
            {
                if (GameData.lobby_keyCollected)
                {
                    Destroy(gameObject);
                    Destroy(imageDisplayGO);
                }
            }
            else if (itemName == PC_Then_Inventory_Objects.Lobby_Torch.ToString())
            {
                if (GameData.lobby_torchCollected)
                {
                    Destroy(gameObject);
                    Destroy(imageDisplayGO);
                }
            }
            else if (itemName == PC_Then_Inventory_Objects.Room1_Barrel.ToString())
            {
                if (GameData.r1_barrelOilCollected)
                {
                    GetComponent<BoxCollider2D>().enabled = false;
                }
            }
        }
        else if(objectData.itemType == ItemType.DescBox_Then_Dialogue)
        {
            DescBox_Then_Dialogue_Object obj = (DescBox_Then_Dialogue_Object)objectData;

            string itemName = obj.sceneName + "_" + obj.itemName;

            if (itemName == Desc_Box_Then_Dialogue_Objects.Room2_Drawer.ToString())
            {
                if (GameData.r2_drawerHealthPotionCollected)
                {
                    GetComponent<BoxCollider2D>().enabled = false;
                }
            }
            else if (itemName == Desc_Box_Then_Dialogue_Objects.Room5_Drawer.ToString())
            {
                if (GameData.r5_drawerHealthPotionCollected)
                {
                    GetComponent<BoxCollider2D>().enabled = false;
                }
            }
        }
        else if(objectData.itemType == ItemType.PC_Then_Note)
        {
            PC_Then_Note_Object obj = (PC_Then_Note_Object)objectData;

            string itemName = obj.sceneName + "_" + obj.itemName;

            if (itemName == PC_Then_Note_Objects.Lobby_Paper.ToString())
            {
                if (GameData.lobby_paperRead)
                {
                    GetComponent<BoxCollider2D>().enabled = false;
                }
            }
            else if (itemName == PC_Then_Note_Objects.Room3_Drawer.ToString())
            {
                if (GameData.r3_drawerNoteRead)
                {
                    GetComponent<BoxCollider2D>().enabled = false;
                }
            }
        }
    }

    public void UpdateGameDataForPCThenInventoryObject()
    {
        PC_Then_Inventory_Object obj = (PC_Then_Inventory_Object)objectData;

        string itemName = obj.sceneName + "_" + obj.itemName;

        if (itemName == PC_Then_Inventory_Objects.Lobby_Torch.ToString())
        {
            GameData.lobby_torchCollected = true;
        }
        else if (itemName == PC_Then_Inventory_Objects.Lobby_Key.ToString())
        {
            // Play key sound
            AudioManager.PlaySoundOnceOnNonPersistentObject(AudioManager.Sound.KeyCollect);

            GameData.lobby_keyCollected = true;
        }
        else if (itemName == PC_Then_Inventory_Objects.Room1_Barrel.ToString())
        {
            GameData.r1_barrelOilCollected = true;
        }
    }

    public void UpdateGameDataForDescBoxThenDialogueObject()
    {
        DescBox_Then_Dialogue_Object obj = (DescBox_Then_Dialogue_Object)objectData;

        string itemName = obj.sceneName + "_" + obj.itemName;

        if (itemName == Desc_Box_Then_Dialogue_Objects.Room2_Drawer.ToString())
        {
            GameData.r2_drawerHealthPotionCollected = true;
        }
        else if (itemName == Desc_Box_Then_Dialogue_Objects.Room5_Drawer.ToString())
        {
            GameData.r5_drawerHealthPotionCollected = true;
        }
    }

    public void UpdateGameDataForPCThenNoteObject()
    {
        PC_Then_Note_Object obj = (PC_Then_Note_Object)objectData;

        string itemName = obj.sceneName + "_" + obj.itemName;

        if (itemName == PC_Then_Note_Objects.Lobby_Paper.ToString())
        {
            GameData.lobby_paperRead = true;
        }
        else if (itemName == PC_Then_Note_Objects.Room3_Drawer.ToString())
        {
            GameData.r3_drawerNoteRead = true;
        }
    }
}
