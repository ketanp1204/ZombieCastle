using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class BossVisibleCameraZoomOut : MonoBehaviour
{
    // Singleton
    public static BossVisibleCameraZoomOut instance;

    private BoxCollider2D bossFightStartCollider;

    public CinemachineVirtualCamera bossCamera;
    public BossAI bossAI;
    public CanvasGroup bossHealthBarCG;

    public GameObject bossCombatBlockWall1;
    public GameObject bossCombatBlockWall2;

    private float dialogueDisplayDelay = 4f;
    [TextArea(3, 6)]
    public string[] dialogueBeforeBossBatlle;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        bossFightStartCollider = GetComponent<BoxCollider2D>();
        bossCombatBlockWall1.SetActive(false);
        bossCombatBlockWall2.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Change to boss camera
            bossCamera.Priority = 15;

            if (!GameData.r5_bossDialogueSeen)
            {
                // Show player dialogue about using knife
                if (DialogueBox.instance)
                {
                    DialogueBox.instance.dialogueDisplayDelay = dialogueDisplayDelay;
                    DialogueBox.instance.SetRoom5BossBattleFlag();
                    DialogueBox.instance.FillSentences(dialogueBeforeBossBatlle);
                    DialogueBox.instance.StartDialogueDisplay();
                }
            }
            else
            {
                StartBossBatlle();
            }

            // Disable this collider
            bossFightStartCollider.enabled = false;
        }
    }

    public void StartBossBatlle()
    {
        // Block player and boss in combat area
        bossCombatBlockWall1.SetActive(true);
        bossCombatBlockWall2.SetActive(true);

        // Start boss chase AI and show his health bar
        bossAI.StartChasingPlayer();
        bossHealthBarCG.alpha = 1f;

        // Set player combat state
        Player.SetCombatState();

        // Change player fireball prefab
        PlayerCombat.instance.fireballPrefab = GameAssets.instance.playerBossFireballParticlePrefab;
    }
}
