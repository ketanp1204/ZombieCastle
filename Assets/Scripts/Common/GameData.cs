using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameData
{
    // Initialized or not check
    public static bool isInitialized = false;

    // Current inventory
    public static InventoryObject currentPlayerInventory;

    // Castle Lobby
    public static bool lobby_introDialogueSeen;
    public static bool lobby_instructionsSeen;
    public static bool lobby_torchCollected;
    public static bool lobby_keyCollected;
    public static bool lobby_paperRead;
    public static bool lobby_r5_stairsUnlocked;
    public static bool lobby_tried_opening_r3_door_with_torch;
    public static bool lobby_opened_r3_door_with_torch;

    // Room 1
    public static bool r1_combatCompleted;
    public static bool r1_mazePuzzleCompleted;
    public static bool r1_treasureBoxAxeCollected;
    public static bool r1_barrelOilCollected;

    // Room 2
    public static bool r2_combatCompleted;
    public static bool r2_jigsawPuzzleCompleted;
    public static bool r2_drawerHealthPotionCollected;
    public static bool r2_treasureBoxMagicPotionCollected;

    // Room 3
    public static bool r3_drawerNoteRead;
    public static bool r3_spotDifferencePuzzleCompleted;
    public static bool r3_treasureBoxFireElementCollected;

    // Room 5
    public static bool r5_zombie3CombatCompleted;
    public static bool r5_drawerHealthPotionCollected;
    public static bool r5_bossDialogueSeen;

    // Scene loading
    public static LevelManager.SceneNames sceneName;
    public static bool loadingCheckpointFromGameOver;

    public static void Initialize()
    {
        currentPlayerInventory = GameAssets.instance.playerStartInventory;
        UnsetBools();
        isInitialized = true;
        sceneName = LevelManager.SceneNames.Lobby;
    }

    public static void ResetData()
    {
        isInitialized = false;
        currentPlayerInventory = null;
        sceneName = LevelManager.SceneNames.Lobby;
        UnsetBools();
    }
    
    private static void UnsetBools()
    {
        lobby_introDialogueSeen = false;            
        lobby_instructionsSeen = false;
        lobby_torchCollected = false;
        lobby_keyCollected = false;
        lobby_paperRead = false;
        lobby_r5_stairsUnlocked = false;             
        lobby_tried_opening_r3_door_with_torch = false;
        lobby_opened_r3_door_with_torch = false;

        r1_combatCompleted = false;
        r1_mazePuzzleCompleted = false;
        r1_treasureBoxAxeCollected = false;
        r1_barrelOilCollected = false;

        r2_combatCompleted = false;
        r2_jigsawPuzzleCompleted = false;
        r2_drawerHealthPotionCollected = false;
        r2_treasureBoxMagicPotionCollected = false;

        r3_drawerNoteRead = false;
        r3_spotDifferencePuzzleCompleted = false;
        r3_treasureBoxFireElementCollected = false;

        r5_zombie3CombatCompleted = false;
        r5_drawerHealthPotionCollected = false;
        r5_bossDialogueSeen = false;

        loadingCheckpointFromGameOver = false;
    }
}
