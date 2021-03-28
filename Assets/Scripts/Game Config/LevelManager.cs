using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public enum SceneNames
    {
        Lobby,
        Room1,
        Room2,
        Room3,
        Room5
    }

    public static LevelManager instance;

    public float transitionTime = 1f;               // Time for which the transition fade animation runs
    public Animator animator;                       // Reference to CrossFade animator

    private float animatorSpeed = 1f;
    private float crossfadeEndAnimLength = 0.5f;
    private float crossfadeStartAnimLength = 0.5f;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetSceneFadeInTime(scene);
        UpdateAnimationClipTimes();

        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("CrossFade_End"))
        {
            animator.SetTrigger("CrossFade_End");
        }
    }

    private void SetSceneFadeInTime(Scene scene)
    {
        if (scene.name == "TitleScreen")
        {
            animatorSpeed = 0.3f;
        }
        else if (scene.name == "IntroSequence")
        {
            animatorSpeed = 0.1f;
        }
        else if (scene.name == "CastleLobby")
        {
            if (PlayerStats.isFirstScene)
            {
                if (GameData.lobby_introDialogueSeen)
                    animatorSpeed = 1f;
                else
                    animatorSpeed = 0.15f;
            }
            else
            {
                animatorSpeed = 1f;
            }
        }
        else
        {
            animatorSpeed = 1f;
        }
    }

    private void UpdateAnimationClipTimes()
    {
        instance.animator.speed = instance.animatorSpeed;
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        crossfadeEndAnimLength = clips[0].length / animatorSpeed; 
        crossfadeStartAnimLength = clips[1].length / animatorSpeed;
    }

    public static void SetAnimatorSpeed(float speed)
    {
        instance.animatorSpeed = speed;
        instance.UpdateAnimationClipTimes();
    }

    public static void FadeScreenInAndOut()
    {
        instance.UpdateAnimationClipTimes();
        instance.StartCoroutine(instance.FadeInOutScreen());
    }

    private IEnumerator FadeInOutScreen()
    {
        instance.UpdateAnimationClipTimes();
        instance.animator.SetTrigger("CrossFade_Start");

        yield return new WaitForSeconds(crossfadeStartAnimLength);

        instance.animator.SetTrigger("CrossFade_End");
    }

    public static void FadeOutScreen()
    {
        instance.UpdateAnimationClipTimes();
        instance.animator.SetTrigger("CrossFade_Start");
    }

    public static void FadeInScreen()
    {
        instance.UpdateAnimationClipTimes();
        instance.animator.SetTrigger("CrossFade_End");
    }

    public static void LoadNextLevel()
    {
        instance.UpdateAnimationClipTimes();
        int sceneToLoad = SceneManager.GetActiveScene().buildIndex + 1;
        instance.StartCoroutine(instance.LoadLevel(sceneToLoad));
    }

    public static void LoadSceneByName(string name)
    {
        instance.UpdateAnimationClipTimes();
        instance.StartCoroutine(instance.LoadLevel(name));
    }

    private IEnumerator LoadLevel(int levelIndex)
    {
        animator.SetTrigger("CrossFade_Start");

        yield return new WaitForSeconds(crossfadeStartAnimLength);

        SceneManager.LoadScene(levelIndex);
    }

    private IEnumerator LoadLevel(string sceneName)
    {
        animator.SetTrigger("CrossFade_Start");

        yield return new WaitForSeconds(crossfadeStartAnimLength);

        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
    }
}
