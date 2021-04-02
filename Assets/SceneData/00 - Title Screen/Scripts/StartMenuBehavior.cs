using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMenuBehavior : MonoBehaviour
{
    // Cached References
    public Animator animator;
    public Button quitButton;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;                                               // Center and lock mouse cursor
        Cursor.lockState = CursorLockMode.None;                                                 // Unlock mouse cursor

        quitButton.onClick.AddListener(() => LevelManager.instance.QuitGame());

        if (!AudioManager.isInitialized)
            AudioManager.Initialize();
        AudioManager.PlaySoundLooping(AudioManager.Sound.TitleScreenTrack);
        AudioManager.StopLoopingSound(AudioManager.Sound.BackgroundTrack);
        AudioManager.StopLoopingSound(AudioManager.Sound.Room5Background);

        GameData.ResetData();
        GameData.Initialize();
        GameData.currentPlayerInventory.Container.Clear();
        GameData.currentPlayerInventory.AddItem(GameAssets.instance.knifeObject, 1);
    }

    public void StartGame()
    {
        // Fade out title screen
        animator.SetTrigger("FadeOut");

        // Get title music AudioSource
        AudioSource titleTrackAudioSource = AudioManager.loopingSoundGameObjects[AudioManager.Sound.TitleScreenTrack].GetComponent<AudioSource>();

        // Fade out title screen music
        new Task(AudioFadeUtils.StartSingleAudioSourceFade(titleTrackAudioSource, 1f, 0f));

        Cursor.lockState = CursorLockMode.Locked;                                               // Center and lock mouse cursor
        LevelManager.LoadNextLevel();
    }
}
