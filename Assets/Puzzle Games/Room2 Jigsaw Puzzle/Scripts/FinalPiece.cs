using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalPiece : MonoBehaviour
{
    public enum FinalPieceIndex
    {
        piece01,
        piece02,
        piece03,
        piece04,
        piece05,
        piece06,
        piece07,
        piece08,
        piece09,
        piece10,
        piece11,
        piece12,
        piece13,
        piece14
    }

    // Private References
    private CircleCollider2D circleCollider;
    private SpriteRenderer spriteRenderer;

    // Public variables
    public FinalPieceIndex pieceIndex;


    // Start is called before the first frame update
    void Start()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        circleCollider.enabled = true;

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// To reset the game
    /// </summary>
    
    public void EnableSpriteRenderer()
    {
        spriteRenderer.enabled = true;
    }

    public void DisableSpriteRenderer()
    {
        spriteRenderer.enabled = false;
    }

    public void EnableCollider()
    {
        circleCollider.enabled = true;
    }

    public void DisableCollider()
    {
        circleCollider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("JigsawPuzzleDraggablePiece"))
        {
            DraggablePiece draggablePieceScript = collision.GetComponent<DraggablePiece>();

            if (draggablePieceScript.draggablePieceIndex.ToString() == pieceIndex.ToString())
            {
                draggablePieceScript.foundFinalPiece = true;
                draggablePieceScript.finalPieceInstance = this;
            }
        }
    }
}
