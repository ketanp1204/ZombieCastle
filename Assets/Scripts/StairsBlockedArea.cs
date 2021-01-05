using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class StairsBlockedArea : MonoBehaviour
{
    public CameraFollow cameraFollow;

    void OnEnable()
    {

        SetCameraBounds(-2.8f, 2.82f, -2.83f, -1.99f);
    }

    public void SetCameraBounds(float xMin, float xMax, float yMin, float yMax)
    {
        // cameraFollow.SetCameraBounds(xMin, xMax, yMin, yMax);   
    }
}
