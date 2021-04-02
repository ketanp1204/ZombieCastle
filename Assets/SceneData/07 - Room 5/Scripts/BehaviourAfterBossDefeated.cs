using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;

public class BehaviourAfterBossDefeated : MonoBehaviour
{
    // Public references
    // Dialogue camera
    public CinemachineVirtualCamera gameEndDialogueCamera;

    // Boss and player health bars
    public CanvasGroup bossHealthBarCG;
    public CanvasGroup playerHealthBarCG;

    // Cage image
    public GameObject cage;

    // Combat block wall
    public GameObject combatBlockWall;

    // Dialogue boxes UI
    public TextMeshProUGUI playerText;
    public TextMeshProUGUI friendText;
    public CanvasGroup playerBoxCG;
    public CanvasGroup friendBoxCG;

    // Player and friend dialogues
    public string[] playerDialogues = { "", ""};
    public string[] friendDialogues = { "", "" } ;

    // Text typing config
    public float typingSpeed = 0.045f;                                                                      // Float - Text auto-type speed in seconds
    public float textSoundEffectPlayInterval = 0.09f;                                                       // Float - Interval between each text type sound effect

    // Private variables
    private bool isTyping = false;

    public void StartGameEndBehaviour()
    {
        new Task(GameEndBehaviour());
    }

    private IEnumerator GameEndBehaviour()
    {
        AudioManager.StopLoopingSound(AudioManager.Sound.Room5Background);
        AudioManager.StopLoopingSound(AudioManager.Sound.BossBattleTrack);

        yield return new WaitForSeconds(1f);

        AudioManager.PlaySoundLooping(AudioManager.Sound.BackgroundTrack);
        AudioManager.SetLoopingSoundVolume(AudioManager.Sound.BackgroundTrack, 0.08f);

        // Hide boss health bar
        bossHealthBarCG.alpha = 0f;

        // Hide player health bar
        playerHealthBarCG.alpha = 0f;

        // Hide combat block wall
        combatBlockWall.SetActive(false);

        // Unequip sword
        Player.UnequipSword();

        // Disable player movement and make him face Betty
        Player.instance.MakePlayerFaceLeft();
        Player.StopMovement();

        // Disable inventory and toolbar manager input
        InventoryManager.DisableInventoryOpen();
        ToolbarManager.DisableToolbarOpen();

        yield return new WaitForSeconds(2f);

        LevelManager.SetAnimatorSpeed(0.4f);
        LevelManager.FadeScreenInAndOut();

        yield return new WaitForSeconds(LevelManager.GetCrossFadeStartAnimLength());

        // Remove cage
        cage.SetActive(false);

        // Remove boss from scene
        Destroy(GameObject.Find("BossZombie"));

        // Set player position near cage
        Player.instance.transform.position = new Vector3(-8.59f, -13.32f, 0f);

        // Show dialogue camera
        gameEndDialogueCamera.Priority = 20;

        yield return new WaitForSeconds(3f);

        // Show player box and first dialogue
        new Task(UIAnimation.FadeCanvasGroupAfterDelay(playerBoxCG, 0f, 1f, 0f, 0.2f));
        playerText.text = "";
        new Task(TypeText(playerText, playerDialogues[0], 0.3f));

        yield return new WaitForSeconds(3f);

        // Hide player box
        new Task(UIAnimation.FadeCanvasGroupAfterDelay(playerBoxCG, 1f, 0f, 0f, 0.4f));

        // Show friend box and first dialogue
        new Task(UIAnimation.FadeCanvasGroupAfterDelay(friendBoxCG, 0f, 1f, 0f, 0.5f));
        friendText.text = "";
        new Task(TypeText(friendText, friendDialogues[0], 0.3f));

        yield return new WaitForSeconds(4f);

        // Show friend second dialogue
        friendText.text = "";
        new Task(TypeText(friendText, friendDialogues[1], 0.3f));

        yield return new WaitForSeconds(3f);

        // Hide friend box
        new Task(UIAnimation.FadeCanvasGroupAfterDelay(friendBoxCG, 1f, 0f, 0f, 0.2f));

        // Show player box and second dialogue
        new Task(UIAnimation.FadeCanvasGroupAfterDelay(playerBoxCG, 0f, 1f, 0f, 0.2f));
        playerText.text = "";
        new Task(TypeText(playerText, playerDialogues[1], 0.3f));

        yield return new WaitForSeconds(3f);

        // Hide player box
        new Task(UIAnimation.FadeCanvasGroupAfterDelay(playerBoxCG, 1f, 0f, 0f, 0.4f));

        // Load game ending scene
        LevelManager.LoadSceneByName("GameEndScene");
    }

    private IEnumerator TypeText(TextMeshProUGUI textMeshProText, string sentence, float delay)                       // Starts typing text after delay
    {
        yield return new WaitForSeconds(delay);

        textMeshProText.text = "";

        isTyping = true;

        new Task(PlayTextTypeSoundEffect());

        foreach (char letter in sentence.ToCharArray())
        {
            AudioManager.PlaySoundOnceOnPersistentObject(AudioManager.Sound.TextAutoTypingSound);
            textMeshProText.text += letter;                                                                                 // Auto-typing text              
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    private IEnumerator PlayTextTypeSoundEffect()
    {
        while (isTyping)
        {
            AudioManager.PlayOneShotSound(AudioManager.Sound.TextAutoTypingSound);
            yield return new WaitForSeconds(textSoundEffectPlayInterval);
        }
    }
}
