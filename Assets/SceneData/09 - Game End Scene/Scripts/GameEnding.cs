using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Experimental.Rendering.Universal;

public class GameEnding : MonoBehaviour
{
    // Public references
    // Dialogue boxes UI
    public TextMeshProUGUI playerText;
    public TextMeshProUGUI friendText;
    public CanvasGroup playerBoxCG;
    public CanvasGroup friendBoxCG;

    // Player and friend dialogues
    public string playerDialogue = "";
    public string friendDialogue = "";

    // Global 2D Light to hide the images after dialogue
    public Light2D globalLight;

    // Game title UI text object
    public TextMeshProUGUI gameTitleText;

    // Credits UI text objects
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI nameText;

    // Credits Texts
    public string[] titles;
    public string[] names;

    // Text typing config
    public float typingSpeed = 0.045f;                                                                      // Float - Text auto-type speed in seconds
    public float textSoundEffectPlayInterval = 0.09f;                                                       // Float - Interval between each text type sound effect

    // Private variables
    private bool isTyping = false;

    private void Start()
    {
        new Task(EndGameBehaviour());
    }

    private IEnumerator EndGameBehaviour()
    {
        yield return new WaitForSeconds(4f);

        // Show friend box and dialogue
        new Task(UIAnimation.FadeCanvasGroupAfterDelay(friendBoxCG, 0f, 1f, 0f, 0.5f));
        friendText.text = "";
        new Task(TypeText(friendText, friendDialogue, 0.3f));

        yield return new WaitForSeconds(4f);

        // Hide friend box
        new Task(UIAnimation.FadeCanvasGroupAfterDelay(friendBoxCG, 1f, 0f, 0f, 0.2f));

        // Show player box and dialogue
        new Task(UIAnimation.FadeCanvasGroupAfterDelay(playerBoxCG, 0f, 1f, 0f, 0.2f));
        playerText.text = "";
        new Task(TypeText(playerText, playerDialogue, 0.3f));

        yield return new WaitForSeconds(4f);

        // Hide player box
        new Task(UIAnimation.FadeCanvasGroupAfterDelay(playerBoxCG, 1f, 0f, 0f, 0.4f));

        LevelManager.FadeScreenInAndOut();

        yield return new WaitForSeconds(LevelManager.GetCrossFadeStartAnimLength());

        // Hide all images
        globalLight.intensity = 0f;

        yield return new WaitForSeconds(2f);

        // Show game title
        new Task(UIAnimation.FadeTMProTextAfterDelay(gameTitleText, 0f, 1f, 0f, 0.4f));

        yield return new WaitForSeconds(3f);

        // Hide game title
        new Task(UIAnimation.FadeTMProTextAfterDelay(gameTitleText, 1f, 0f, 0f, 0.4f));

        yield return new WaitForSeconds(4f);

        // Game credits
        // Project coordinator
        titleText.text = titles[0];
        new Task(UIAnimation.FadeTMProTextAfterDelay(titleText, 0f, 1f, 0f, 0.4f));

        yield return new WaitForSeconds(1.5f);

        // Gianluca Pandolfo
        nameText.text = names[0];
        new Task(UIAnimation.FadeTMProTextAfterDelay(nameText, 0f, 1f, 0f, 0.4f));

        yield return new WaitForSeconds(3f);

        // Hide name and title texts
        new Task(UIAnimation.FadeTMProTextAfterDelay(titleText, 1f, 0f, 0f, 0.4f));
        new Task(UIAnimation.FadeTMProTextAfterDelay(nameText, 1f, 0f, 0f, 0.4f));

        yield return new WaitForSeconds(2.5f);

        // Sound and Narrative Designer
        titleText.text = titles[1];
        new Task(UIAnimation.FadeTMProTextAfterDelay(titleText, 0f, 1f, 0f, 0.4f));

        yield return new WaitForSeconds(1.5f);

        // Abhay Syal
        nameText.text = names[1];
        new Task(UIAnimation.FadeTMProTextAfterDelay(nameText, 0f, 1f, 0f, 0.4f));

        yield return new WaitForSeconds(3f);

        // Hide name and title texts
        new Task(UIAnimation.FadeTMProTextAfterDelay(titleText, 1f, 0f, 0f, 0.4f));
        new Task(UIAnimation.FadeTMProTextAfterDelay(nameText, 1f, 0f, 0f, 0.4f));

        yield return new WaitForSeconds(2.5f);

        // Character Design and UI
        titleText.text = titles[2];
        new Task(UIAnimation.FadeTMProTextAfterDelay(titleText, 0f, 1f, 0f, 0.4f));

        yield return new WaitForSeconds(1.5f);

        // Junting Liu
        nameText.text = names[2];
        new Task(UIAnimation.FadeTMProTextAfterDelay(nameText, 0f, 1f, 0f, 0.4f));

        yield return new WaitForSeconds(3f);

        // Hide name and title texts
        new Task(UIAnimation.FadeTMProTextAfterDelay(titleText, 1f, 0f, 0f, 0.4f));
        new Task(UIAnimation.FadeTMProTextAfterDelay(nameText, 1f, 0f, 0f, 0.4f));

        yield return new WaitForSeconds(2.5f);

        // Game Art Design
        titleText.text = titles[3];
        new Task(UIAnimation.FadeTMProTextAfterDelay(titleText, 0f, 1f, 0f, 0.4f));

        yield return new WaitForSeconds(1.5f);

        // Jin Wang
        nameText.text = names[3];
        new Task(UIAnimation.FadeTMProTextAfterDelay(nameText, 0f, 1f, 0f, 0.4f));

        yield return new WaitForSeconds(3f);

        // Hide name and title texts
        new Task(UIAnimation.FadeTMProTextAfterDelay(titleText, 1f, 0f, 0f, 0.4f));
        new Task(UIAnimation.FadeTMProTextAfterDelay(nameText, 1f, 0f, 0f, 0.4f));

        yield return new WaitForSeconds(2.5f);

        // Game Programming and Logic
        titleText.text = titles[4];
        new Task(UIAnimation.FadeTMProTextAfterDelay(titleText, 0f, 1f, 0f, 0.4f));

        yield return new WaitForSeconds(1.5f);

        // Ketan Patel
        nameText.text = names[4];
        new Task(UIAnimation.FadeTMProTextAfterDelay(nameText, 0f, 1f, 0f, 0.4f));

        yield return new WaitForSeconds(3f);

        // Hide name and title texts
        new Task(UIAnimation.FadeTMProTextAfterDelay(titleText, 1f, 0f, 0f, 0.4f));
        new Task(UIAnimation.FadeTMProTextAfterDelay(nameText, 1f, 0f, 0f, 0.4f));

        yield return new WaitForSeconds(2.5f);

        LevelManager.LoadSceneByName("TitleScreen");
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
