using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
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
        if (scene.name == "StartScene")
        {
            animatorSpeed = 0.3f;
            animator.speed = animatorSpeed;
        }
        else if (scene.name == "IntroSequence")
        {
            animatorSpeed = 0.1f;
            animator.speed = animatorSpeed;
        }
        else if (scene.name == "CastleLobby")
        {
            animatorSpeed = 0.15f;
            animator.speed = animatorSpeed;
        }
    }

    private void UpdateAnimationClipTimes()
    {
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        crossfadeEndAnimLength = clips[0].length / animatorSpeed; 
        crossfadeStartAnimLength = clips[1].length / animatorSpeed;
    }

    public static void SetAnimatorSpeed(float speed)
    {
        instance.animatorSpeed = speed;
        instance.animator.speed = instance.animatorSpeed;
        instance.UpdateAnimationClipTimes();
    }

    public static void FadeScreenInAndOut()
    {
        instance.StartCoroutine(instance.FadeInOutScreen());
    }

    private IEnumerator FadeInOutScreen()
    {
        instance.animator.SetTrigger("CrossFade_Start");

        yield return new WaitForSeconds(crossfadeStartAnimLength);

        instance.animator.SetTrigger("CrossFade_End");
    }

    public static void FadeOutScreen()
    {
        instance.animator.SetTrigger("CrossFade_Start");
    }

    public static void FadeInScreen()
    {
        instance.animator.SetTrigger("CrossFade_End");
    }

    public static void LoadNextLevel()
    {
        int sceneToLoad = SceneManager.GetActiveScene().buildIndex + 1;
        instance.StartCoroutine(instance.LoadLevel(sceneToLoad));
    }

    public static void LoadSceneByName(string name)
    {
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

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
    }
}
