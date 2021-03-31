using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class LobbyStairsBlock : MonoBehaviour
{
    // Private references
    private BoxCollider2D triggerCollider;
    private SpriteGlow.SpriteGlowEffect glowEffect;
    private SpriteRenderer outlineSR;

    // Public references
    public BoxCollider2D stairsBlockCollider;
    public GameObject stairsBlockDisplayGO;
    public CinemachineVirtualCamera topFloorCamera;

    // Start is called before the first frame update
    void Start()
    {
        SetReferences();
        Initialize();
    }

    private void SetReferences()
    {
        triggerCollider = GetComponent<BoxCollider2D>();
        glowEffect = GetComponent<SpriteGlow.SpriteGlowEffect>();
        outlineSR = GetComponent<SpriteRenderer>();
    }

    private void Initialize()
    {
        glowEffect.enabled = false;

        if (GameData.lobby_r5_stairsUnlocked)
        {
            // Enable stairs unlock interaction
            triggerCollider.enabled = true;
        }
        else
        {
            // Disable stairs unlock interaction
            triggerCollider.enabled = false;
        }
    }

    public void UnblockStairs()
    {
        topFloorCamera.Priority = 15;

        outlineSR.enabled = false;

        stairsBlockCollider.enabled = false;

        stairsBlockDisplayGO.SetActive(false);

        triggerCollider.enabled = false;
    }
}
