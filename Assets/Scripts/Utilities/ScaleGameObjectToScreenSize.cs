using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleGameObjectToScreenSize : MonoBehaviour
{
    private readonly int referenceResolutionX = 1920;
    private readonly int referenceResolutionY = 1080;

    private int currentResolutionX;
    private int currentResolutionY;

    // Start is called before the first frame update
    void Start()
    {
        currentResolutionX = Screen.width;
        currentResolutionY = Screen.height;

        float finalScaleX;
        float finalScaleY;
        float scaleCorrectionMultiplier;

        float scaleX = (float)currentResolutionX / (float)referenceResolutionX;
        float scaleY = (float)currentResolutionY / (float)referenceResolutionY;

        scaleCorrectionMultiplier = 1 / scaleY;

        finalScaleX = scaleX * scaleCorrectionMultiplier;
        finalScaleY = scaleY * scaleCorrectionMultiplier;

        gameObject.transform.localScale = new Vector3(finalScaleX, finalScaleY, 1f);
    }
}
