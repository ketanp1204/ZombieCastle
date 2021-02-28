using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public static class UIAnimation
{
    // Coroutine to fade canvas group
    public static IEnumerator FadeCanvasGroupAfterDelay(CanvasGroup canvasGroup, float startAlpha, float endAlpha, float delay, float lerpTime = 0.3f)  
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

            if (canvasGroup != null)
            {
                canvasGroup.alpha = currentValue;
            }

            if (percentageComplete >= 1)
            {
                break;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    public static IEnumerator FadeTMProTextAfterDelay(TextMeshProUGUI text, float startAlpha, float endAlpha, float delay, float lerpTime = 0.3f)
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

            if (text != null)
            {
                text.alpha = currentValue;
            }

            if (percentageComplete >= 1)
            {
                break;
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
