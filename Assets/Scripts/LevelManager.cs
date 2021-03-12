using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public float transitionTime = 0.5f;             // Time for which the transition fade animation runs
    public Animator animator;                       // Reference to CrossFade animator
    private string sceneToLoad;                     // Scene to which the transition will go to

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
        StartCoroutine(WaitForSceneToLoad());
    }

    public static void FadeScreenInAndOut()
    {
        instance.StartCoroutine(instance.FadeInOutScreen());
    }

    private IEnumerator FadeInOutScreen()
    {
        instance.animator.SetTrigger("CrossFade_Start");

        yield return new WaitForSeconds(0.5f);

        instance.animator.SetTrigger("CrossFade_End");
    }

    private IEnumerator WaitForSceneToLoad()
    {
        yield return new WaitForSeconds(0.5f);

        if(!animator.GetCurrentAnimatorStateInfo(0).IsName("CrossFade_End"))
        {
            animator.SetTrigger("CrossFade_End");
        }
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

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(levelIndex);
    }

    private IEnumerator LoadLevel(string sceneName)
    {
        animator.SetTrigger("CrossFade_Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(sceneName);
    }
}
