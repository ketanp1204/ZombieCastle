using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverScreen : MonoBehaviour
{
    // Public References
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI loadCheckpointText;
    public TextMeshProUGUI quitGameText;

    // Public variables
    public float gameOverTextAnimLerpTime;

    // Start is called before the first frame update
    void Start()
    {
        // Lock the cursor
        Cursor.lockState = CursorLockMode.Locked;

        gameOverText.alpha = 1f;
        loadCheckpointText.alpha = 0f;
        quitGameText.alpha = 0f;

        new Task(StartGameOverBehaviour());
    }

    private IEnumerator StartGameOverBehaviour()
    {
        new Task(UIAnimation.InterpolateTMProTextSizeAfterDelay(gameOverText, 250f, 150f, 0f, gameOverTextAnimLerpTime));

        yield return new WaitForSeconds(0.15f);

        // Play game over sound
        AudioManager.PlaySoundOnceOnNonPersistentObject(AudioManager.Sound.GameOver);

        yield return new WaitForSeconds(2f);

        new Task(UIAnimation.FadeTMProTextAfterDelay(loadCheckpointText, 0f, 1f, 0f, 0.5f));
        new Task(UIAnimation.FadeTMProTextAfterDelay(quitGameText, 0f, 1f, 0f, 0.5f));

        yield return new WaitForSeconds(0.5f);

        // Enable cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.lockState = CursorLockMode.None;
    }

    public static void LoadLastCheckpoint()
    {
        GameData.loadingCheckpointFromGameOver = true;

        PlayerStats.IsDead = false;

        if (GameData.sceneName == LevelManager.SceneNames.Lobby)
        {
            LevelManager.LoadSceneByName("CastleLobby");
        }
        else if (GameData.sceneName == LevelManager.SceneNames.Room1)
        {
            LevelManager.LoadSceneByName("Room1");
        }
        else if (GameData.sceneName == LevelManager.SceneNames.Room2)
        {
            LevelManager.LoadSceneByName("Room2");
        }
        else if (GameData.sceneName == LevelManager.SceneNames.Room3)
        {
            LevelManager.LoadSceneByName("Room3");
        }
        else if (GameData.sceneName == LevelManager.SceneNames.Room5)
        {
            LevelManager.LoadSceneByName("Room5");
        }
    }
}
