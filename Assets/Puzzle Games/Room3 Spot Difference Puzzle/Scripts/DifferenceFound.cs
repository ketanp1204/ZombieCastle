using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifferenceFound : MonoBehaviour
{
    private SpriteGlow.SpriteGlowEffect glowEffect;
    private Collider2D col;

    private void Start()
    {
        glowEffect = GetComponent<SpriteGlow.SpriteGlowEffect>();
        glowEffect.enabled = false;
    }

    private void OnMouseDown()
    {
        col.enabled = false;

        glowEffect.enabled = true;

        SpotDifferencesPuzzle.instance.AddOneDifferenceFound();
    }

    public void EnableCollider()
    {
        col = GetComponent<Collider2D>();
        col.enabled = true;
    }

    public void DisableCollider()
    {
        col = GetComponent<Collider2D>();
        col.enabled = false;
    }

    public void Reset()
    {
        glowEffect.enabled = false;
        col.enabled = false;
    }
}
