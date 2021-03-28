using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class BossVisibleCameraZoomOut : MonoBehaviour
{

    public CinemachineVirtualCamera bossCamera;
    public BossAI bossAI;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (transform.position.x < collision.transform.position.x)
        {
            bossCamera.Priority = 15;
            // bossAI.StartChasingPlayer();
        }
        else
        {
            bossCamera.Priority = 5;
            // bossAI.StopMovement();
        }
    }
}
