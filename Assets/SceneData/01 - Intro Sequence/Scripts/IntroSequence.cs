﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;
using UnityEngine.Experimental.Rendering.Universal;

public class IntroSequence : MonoBehaviour
{
    [Header("General Config")]
    public float typingSpeed = 0.045f;                                                                  // Float - Text auto-type speed in seconds
    public TextMeshProUGUI skipIntroText;
    public float skipIntroKeyHoldTime;

    [Header("Background Images and Camera")]
    public Transform img1_CamStartPos;
    public Transform img2_CamStartPos;
    public Transform img3_CamStartPos;
    public Transform img4_CamStartPos;
    public Transform img5_CamStartPos;
    public PolygonCollider2D img1_Bounds;
    public PolygonCollider2D img2_Bounds;
    public PolygonCollider2D img3_Bounds;
    public PolygonCollider2D img4_Bounds;
    public PolygonCollider2D img5_Bounds;
    public CinemachineVirtualCamera cinemachineCamera;
    public SpriteRenderer img5_SR;

    [Header("Image1")]
    public SpriteRenderer img1_PlayerSR;
    public SpriteRenderer img1_FriendSR;
    public TextMeshProUGUI img1_PlayerText;
    public TextMeshProUGUI img1_FriendText;
    public CanvasGroup img1_PlayerBox;
    public CanvasGroup img1_FriendBox;
    public string img1_PlayerDialogue = "";
    public string[] img1_FriendDialogues = { "", "" };

    [Header("Image2")]
    public TextMeshProUGUI img2_PlayerText;
    public TextMeshProUGUI img2_FriendText;
    public CanvasGroup img2_PlayerBox;
    public CanvasGroup img2_FriendBox;
    public string img2_PlayerDialogue = "";
    public string[] img2_FriendDialogues = { "", "" };

    [Header("Image3")]
    public TextMeshProUGUI img3_PlayerText;
    public TextMeshProUGUI img3_FriendText;
    public CanvasGroup img3_PlayerBox;
    public CanvasGroup img3_FriendBox;
    public string[] img3_PlayerDialogues = { "", "", "" };
    public string img3_FriendDialogue = "";

    [Header("Image4")]
    public TextMeshProUGUI img4_PlayerText;
    public TextMeshProUGUI img4_FriendText;
    public CanvasGroup img4_PlayerBox;
    public CanvasGroup img4_FriendBox;
    public string[] img4_PlayerDialogues = { "", ""};
    public string[] img4_FriendDialogues = { "", "" };

    [Header("Image5")]
    public TextMeshProUGUI img5_PlayerText;
    public TextMeshProUGUI img5_FriendText;
    public CanvasGroup img5_PlayerBox;
    public CanvasGroup img5_FriendBox;
    public string[] img5_PlayerDialogues = { "", "" };
    public string img5_FriendDialogue = "";
    public GameObject img5_fogGameObject;
    public Light2D pumpkinLight2D;
    public SpriteRenderer colorImage;

    [Header("Outro Text")]
    public TextMeshProUGUI outroTMPro;
    public string outroText;

    public float textSoundEffectPlayInterval = 0.09f;                                                    // Float - Interval between each text type sound effect

    // Private variables
    private bool isTyping = false;
    private bool isActive = false;
    private bool skipIntroHoldOver = false;
    private bool checkForSpacePress = false;
    private bool checkForSpaceHold = false;
    private bool spaceKeyDown = false;
    private bool spaceKeyHold = false;

    private float _timeSinceStartedHoldingSpace;
    private float timeSinceStartedHoldingSpace;
    private float percentageComplete;

    private Task task;

    // Start is called before the first frame update
    void Start()
    {
        AudioManager.Initialize();
        isActive = true;
        task = new Task(LoadIntroSequence());
        StartCoroutine(SkipIntroCheck());

        // Cursor.lockState = CursorLockMode.Locked;                                               // Center and lock mouse cursor
    }

    private void Update()
    {
        if (checkForSpacePress)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                spaceKeyDown = true;
            }
        }

        if (checkForSpaceHold)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                spaceKeyHold = true;
            }
            else
            {
                spaceKeyHold = false;
            }
        }
    }

    private IEnumerator SkipIntroCheck()
    {
        while (!skipIntroHoldOver && isActive)
        {
            checkForSpacePress = true;
            while (true)
            {                
                if (spaceKeyDown)
                {
                    // Store space key down time
                    _timeSinceStartedHoldingSpace = Time.time;

                    // Stop checking for space down
                    checkForSpacePress = false;

                    // Start checking for space hold
                    checkForSpaceHold = true;

                    // Unset spaceKeyDown bool
                    spaceKeyDown = false;

                    // Show skip intro text
                    new Task(UIAnimation.FadeTMProTextAfterDelay(skipIntroText, 0f, 1f, 0f));

                    // Hide skip intro text after hold time
                    new Task(UIAnimation.FadeTMProTextAfterDelay(skipIntroText, 1f, 0f, skipIntroKeyHoldTime));

                    break;
                }
                yield return new WaitForEndOfFrame();
            }

            yield return new WaitForEndOfFrame();

            while (spaceKeyHold)
            {
                timeSinceStartedHoldingSpace = Time.time - _timeSinceStartedHoldingSpace;
                percentageComplete = timeSinceStartedHoldingSpace / skipIntroKeyHoldTime;

                if (percentageComplete >= 1)
                {
                    // Stop checking for space hold
                    checkForSpaceHold = false;

                    // Unset spaceKeyHold bool
                    spaceKeyHold = false;

                    // Set skip hold success flag
                    skipIntroHoldOver = true;

                    break;
                }

                yield return new WaitForEndOfFrame();
            }
        }

        // Get background music AudioSource
        AudioSource backgroundMusicAudioSource = AudioManager.loopingSoundGameObjects[AudioManager.Sound.IntroSequenceBackground].GetComponent<AudioSource>();

        // Fade out title screen music
        new Task(AudioFadeUtils.StartSingleAudioSourceFade(backgroundMusicAudioSource, 1f, 0f));

        // Stop intro animation task
        task.Stop();

        // Load castle lobby scene
        LevelManager.LoadNextLevel();
    }

    private IEnumerator LoadIntroSequence()
    {
        // Set crossfade animator speed to half
        LevelManager.SetAnimatorSpeed(0.5f);

        // Play background music
        AudioManager.PlaySoundLooping(AudioManager.Sound.IntroSequenceBackground);
        
        // Set camera to first image
        cinemachineCamera.Follow = img1_CamStartPos;
        cinemachineCamera.GetComponent<CinemachineConfiner>().m_BoundingShape2D = img1_Bounds;

        // Zoom in and out cinemachine camera
        new Task(InterpolateCinemachineCameraSize(4.2f, 5.35f, 0f, 5f));

        yield return new WaitForSeconds(4f);

        // Play character grass walk sound
        AudioManager.PlaySoundOnceOnNonPersistentObject(AudioManager.Sound.IntroSequenceCharacterGrassWalk);

        // Show player and friend approaching
        new Task(FadeSpriteRendererAfterDelay(img1_PlayerSR, 0f, 1f, 0f, 4.519f));
        new Task(FadeSpriteRendererAfterDelay(img1_FriendSR, 0f, 1f, 0f, 4.519f));

        yield return new WaitForSeconds(4.519f);

        // Show friend box and first dialogue
        new Task(UIAnimation.FadeCanvasGroupAfterDelay(img1_FriendBox, 0f, 1f, 0f, 0.5f));
        img1_FriendText.text = "";
        new Task(TypeText(img1_FriendText, img1_FriendDialogues[0], 0.3f));

        yield return new WaitForSeconds(4f);

        // Show friend second dialogue
        img1_FriendText.text = "";
        new Task(TypeText(img1_FriendText, img1_FriendDialogues[1], 0.3f));

        yield return new WaitForSeconds(4f);

        // Hide friend box
        new Task(UIAnimation.FadeCanvasGroupAfterDelay(img1_FriendBox, 1f, 0f, 0f, 0.2f));

        // Show player box and dialogue
        new Task(UIAnimation.FadeCanvasGroupAfterDelay(img1_PlayerBox, 0f, 1f, 0f, 0.2f));
        img1_PlayerText.text = "";
        new Task(TypeText(img1_PlayerText, img1_PlayerDialogue, 0.3f));

        yield return new WaitForSeconds(4f);

        // Hide player box
        new Task(UIAnimation.FadeCanvasGroupAfterDelay(img1_PlayerBox, 1f, 0f, 0f, 0.4f));

        // Fade in and out screen
        LevelManager.FadeScreenInAndOut();

        yield return new WaitForSeconds(1f);

        // Shift to second image
        cinemachineCamera.Follow = img2_CamStartPos;
        cinemachineCamera.GetComponent<CinemachineConfiner>().m_BoundingShape2D = img2_Bounds;

        // Start moving camera towards right
        new Task(MoveCameraFollowTargetX(img2_CamStartPos, img2_CamStartPos.position.x, img2_CamStartPos.position.x + 3.44f, 0f, 6f));

        // Play character grass walk sound
        AudioManager.PlaySoundOnceOnNonPersistentObject(AudioManager.Sound.IntroSequenceCharacterGrassWalk);

        yield return new WaitForSeconds(2f);

        // Show friend box and dialogues
        new Task(UIAnimation.FadeCanvasGroupAfterDelay(img2_FriendBox, 0f, 1f, 0f, 0.5f));
        img2_FriendText.text = "";
        new Task(TypeText(img2_FriendText, img2_FriendDialogues[0], 0.3f));
        yield return new WaitForSeconds(3f);
        img2_FriendText.text = "";
        new Task(TypeText(img2_FriendText, img2_FriendDialogues[1], 0.3f));

        yield return new WaitForSeconds(3f);

        // Hide friend box
        new Task(UIAnimation.FadeCanvasGroupAfterDelay(img2_FriendBox, 1f, 0f, 0f, 0.5f));

        // Show player box and dialogue
        new Task(UIAnimation.FadeCanvasGroupAfterDelay(img2_PlayerBox, 0f, 1f, 0.4f, 0.5f));
        img2_PlayerText.text = "";
        new Task(TypeText(img2_PlayerText, img2_PlayerDialogue, 0.7f));

        yield return new WaitForSeconds(5f);

        // Hide player box
        new Task(UIAnimation.FadeCanvasGroupAfterDelay(img2_PlayerBox, 1f, 0f, 0f, 0.4f));

        // Fade in and out screen
        LevelManager.FadeScreenInAndOut();

        yield return new WaitForSeconds(1f);

        // Shift to third image
        cinemachineCamera.Follow = img3_CamStartPos;
        cinemachineCamera.GetComponent<CinemachineConfiner>().m_BoundingShape2D = img3_Bounds;

        // Zoom in and out cinemachine camera
        new Task(InterpolateCinemachineCameraSize(4.2f, 5.35f, 0f, 5f));

        yield return new WaitForSeconds(1f);

        // Play intro scare sound
        AudioManager.PlaySoundOnceOnNonPersistentObject(AudioManager.Sound.IntroScareSound);

        yield return new WaitForSeconds(2f);

        // Show player box and dialogue
        new Task(UIAnimation.FadeCanvasGroupAfterDelay(img3_PlayerBox, 0f, 1f, 0.4f, 0.5f));
        img3_PlayerText.text = "";
        new Task(TypeText(img3_PlayerText, img3_PlayerDialogues[0], 0.7f));

        yield return new WaitForSeconds(3f);

        img3_PlayerText.text = "";
        new Task(TypeText(img3_PlayerText, img3_PlayerDialogues[1], 0.7f));

        yield return new WaitForSeconds(3f);

        // Hide player box
        new Task(UIAnimation.FadeCanvasGroupAfterDelay(img3_PlayerBox, 1f, 0f, 0f, 0.4f));

        // Show friend box and dialogue
        new Task(UIAnimation.FadeCanvasGroupAfterDelay(img3_FriendBox, 0f, 1f, 0.4f, 0.5f));
        img3_FriendText.text = "";
        new Task(TypeText(img3_FriendText, img3_FriendDialogue, 0.7f));

        yield return new WaitForSeconds(3f);

        // Hide friend box
        new Task(UIAnimation.FadeCanvasGroupAfterDelay(img3_FriendBox, 1f, 0f, 0f, 0.5f));

        // Show player box and dialogue
        new Task(UIAnimation.FadeCanvasGroupAfterDelay(img3_PlayerBox, 0f, 1f, 0.4f, 0.5f));
        img3_PlayerText.text = "";
        new Task(TypeText(img3_PlayerText, img3_PlayerDialogues[2], 0.7f));

        yield return new WaitForSeconds(4f);

        // Hide player box
        new Task(UIAnimation.FadeCanvasGroupAfterDelay(img3_PlayerBox, 1f, 0f, 0f, 0.4f));

        // Fade in and out screen
        LevelManager.FadeScreenInAndOut();

        yield return new WaitForSeconds(1f);

        // Shift to fourth image
        cinemachineCamera.Follow = img4_CamStartPos;
        cinemachineCamera.GetComponent<CinemachineConfiner>().m_BoundingShape2D = img4_Bounds;

        // Zoom in and out cinemachine camera
        new Task(InterpolateCinemachineCameraSize(4.2f, 5.35f, 0f, 5f));

        yield return new WaitForSeconds(2f);

        // Show player box and dialogue
        new Task(UIAnimation.FadeCanvasGroupAfterDelay(img4_PlayerBox, 0f, 1f, 0.4f, 0.5f));
        img4_PlayerText.text = "";
        new Task(TypeText(img4_PlayerText, img4_PlayerDialogues[0], 0.7f));

        yield return new WaitForSeconds(3f);

        // Hide player box
        new Task(UIAnimation.FadeCanvasGroupAfterDelay(img4_PlayerBox, 1f, 0f, 0f, 0.4f));

        // Show friend box and dialogue
        new Task(UIAnimation.FadeCanvasGroupAfterDelay(img4_FriendBox, 0f, 1f, 0.4f, 0.5f));
        img4_FriendText.text = "";
        new Task(TypeText(img4_FriendText, img4_FriendDialogues[0], 0.7f));
        yield return new WaitForSeconds(2.5f);
        img4_FriendText.text = "";
        new Task(TypeText(img4_FriendText, img4_FriendDialogues[1], 0.7f));

        yield return new WaitForSeconds(3f);

        // Hide friend box
        new Task(UIAnimation.FadeCanvasGroupAfterDelay(img4_FriendBox, 1f, 0f, 0f, 0.5f));

        // Show player box and dialogue
        new Task(UIAnimation.FadeCanvasGroupAfterDelay(img4_PlayerBox, 0f, 1f, 0.4f, 0.5f));
        img4_PlayerText.text = "";
        new Task(TypeText(img4_PlayerText, img4_PlayerDialogues[1], 0.7f));

        yield return new WaitForSeconds(7f);

        // Hide player box
        new Task(UIAnimation.FadeCanvasGroupAfterDelay(img4_PlayerBox, 1f, 0f, 0f, 0.4f));

        // Fade in and out screen
        LevelManager.FadeScreenInAndOut();

        yield return new WaitForSeconds(1f);
        
        // Shift to fifth image
        cinemachineCamera.Follow = img5_CamStartPos;
        cinemachineCamera.GetComponent<CinemachineConfiner>().m_BoundingShape2D = img5_Bounds;

        // Zoom in and out cinemachine camera
        new Task(InterpolateCinemachineCameraSize(4.2f, 5.35f, 0f, 2.5f));

        yield return new WaitForSeconds(2f);

        // Show player box and dialogue
        new Task(UIAnimation.FadeCanvasGroupAfterDelay(img5_PlayerBox, 0f, 1f, 0.4f, 0.5f));
        img5_PlayerText.text = "";
        new Task(TypeText(img5_PlayerText, img5_PlayerDialogues[0], 0.7f));

        yield return new WaitForSeconds(1f);

        // Play blackout music
        AudioManager.PlaySoundOnceOnNonPersistentObject(AudioManager.Sound.IntroSequenceBlackout);

        yield return new WaitForSeconds(2f);

        // Start fading in color image
        new Task(FadeSpriteRendererAfterDelay(colorImage, 0f, 0.9f, 0f, 5f));

        // Start increasing pumpkins light intensity
        new Task(InterpolateLight2DIntensity(pumpkinLight2D, pumpkinLight2D.intensity, 15f, 0f, 2f));

        // Get background music AudioSource
        AudioSource backgroundMusicAudioSource = AudioManager.loopingSoundGameObjects[AudioManager.Sound.IntroSequenceBackground].GetComponent<AudioSource>();

        // Fade out title screen music
        new Task(AudioFadeUtils.StartSingleAudioSourceFade(backgroundMusicAudioSource, 1f, 0f));

        // Start zooming in the cinemachine camera
        new Task(InterpolateCinemachineCameraSize(5.35f, 3.8f, 0f, 6f));

        // Hide player box
        new Task(UIAnimation.FadeCanvasGroupAfterDelay(img5_PlayerBox, 1f, 0f, 0f, 0.4f));

        // Show friend box and dialogue
        new Task(UIAnimation.FadeCanvasGroupAfterDelay(img5_FriendBox, 0f, 1f, 0.4f, 0.5f));
        img5_FriendText.text = "";
        new Task(TypeText(img5_FriendText, img5_FriendDialogue, 0.7f));

        yield return new WaitForSeconds(3f);

        // Hide friend box
        new Task(UIAnimation.FadeCanvasGroupAfterDelay(img5_FriendBox, 1f, 0f, 0f, 0.5f));

        // Show player box and dialogue
        new Task(UIAnimation.FadeCanvasGroupAfterDelay(img5_PlayerBox, 0f, 1f, 0.4f, 0.5f));
        img5_PlayerText.text = "";
        new Task(TypeText(img5_PlayerText, img5_PlayerDialogues[1], 0.7f));

        yield return new WaitForSeconds(3f);

        // Hide player box
        new Task(UIAnimation.FadeCanvasGroupAfterDelay(img5_PlayerBox, 1f, 0f, 0f, 0.4f));

        // Fade out screen
        LevelManager.FadeOutScreen();

        // Hide color image
        new Task(FadeSpriteRendererAfterDelay(colorImage, 0.9f, 0f, 1f, 0.1f));

        // Hide pumpkin light
        pumpkinLight2D.intensity = 0f;

        // Remove fog 
        Destroy(img5_fogGameObject);

        yield return new WaitForSeconds(1f);

        // Fade out screen
        LevelManager.FadeInScreen();

        // Black out image 5 sprite renderer
        Color c = img5_SR.color;
        c.r = c.g = c.b = 0f;
        img5_SR.color = c;

        yield return new WaitForSeconds(4f);

        cinemachineCamera.m_Lens.OrthographicSize = 5.35f;

        // Show outro text
        outroTMPro.text = outroText;
        new Task(UIAnimation.FadeTMProTextAfterDelay(outroTMPro, 0f, 1f, 0f));

        yield return new WaitForSeconds(3f);

        // Fade out outro text
        new Task(UIAnimation.FadeTMProTextAfterDelay(outroTMPro, 1f, 0f, 0f, 0.4f));

        yield return new WaitForSeconds(2.5f);

        LevelManager.SetAnimatorSpeed(0.3f);
        LevelManager.LoadNextLevel();
    }

    private IEnumerator InterpolateCinemachineCameraSize(float startSize, float endSize, float delay, float lerpTime = 4f)
    {
        yield return new WaitForSeconds(delay);

        float _timeStartedLerping = Time.time;
        float timeSinceStarted;
        float percentageComplete;

        while (true)
        {
            timeSinceStarted = Time.time - _timeStartedLerping;
            percentageComplete = timeSinceStarted / lerpTime;

            float currentValue = Mathf.Lerp(startSize, endSize, percentageComplete);

            if (cinemachineCamera != null)
            {
                cinemachineCamera.m_Lens.OrthographicSize = currentValue;
            }

            if (percentageComplete >= 1)
            {
                break;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator MoveCameraFollowTargetX(Transform target, float startX, float endX, float delay, float lerpTime = 0.3f)
    {
        yield return new WaitForSeconds(delay);

        float _timeStartedLerping = Time.time;
        float timeSinceStarted;
        float percentageComplete;        

        while (true)
        {
            timeSinceStarted = Time.time - _timeStartedLerping;
            percentageComplete = timeSinceStarted / lerpTime;

            float currentValue = Mathf.Lerp(startX, endX, percentageComplete);
            
            if (target != null)
            {
                float deltaX = currentValue - target.position.x;
                target.position = new Vector3(target.position.x + deltaX, target.position.y, target.position.z);
            }

            if (percentageComplete >= 1)
            {
                break;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator InterpolateLight2DIntensity(Light2D light, float startIntensity, float endIntensity, float delay, float lerpTime = 4f)
    {
        yield return new WaitForSeconds(delay);

        float _timeStartedLerping = Time.time;
        float timeSinceStarted;
        float percentageComplete;

        while (true)
        {
            timeSinceStarted = Time.time - _timeStartedLerping;
            percentageComplete = timeSinceStarted / lerpTime;

            float currentValue = Mathf.Lerp(startIntensity, endIntensity, percentageComplete);

            if (light != null)
            {
                light.intensity = currentValue;
            }

            if (percentageComplete >= 1)
            {
                break;
            }

            yield return new WaitForEndOfFrame();
        }
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

    public static IEnumerator FadeSpriteRendererAfterDelay(SpriteRenderer spriteRenderer, float startAlpha, float endAlpha, float delay, float lerpTime = 0.3f)
    {
        yield return new WaitForSeconds(delay);

        float _timeStartedLerping = Time.time;
        float timeSinceStarted;
        float percentageComplete;

        while (true)
        {
            timeSinceStarted = Time.time - _timeStartedLerping;
            percentageComplete = timeSinceStarted / lerpTime;

            float currentValue = Mathf.Lerp(startAlpha, endAlpha, percentageComplete);

            if (spriteRenderer != null)
            {
                Color c = spriteRenderer.color;
                c.a = currentValue;
                spriteRenderer.color = c;
            }

            if (percentageComplete >= 1)
            {
                break;
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
