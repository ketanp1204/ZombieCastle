using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class BossVisibleCameraZoomOut : MonoBehaviour
{

    public CinemachineVirtualCamera bossCamera;

    private bool cameraZoomedOut = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (cameraZoomedOut)
        {
            bossCamera.Priority = 5;
            cameraZoomedOut = false;
        }
        else
        {
            bossCamera.Priority = 15;
            cameraZoomedOut = true;
        }
    }
}
