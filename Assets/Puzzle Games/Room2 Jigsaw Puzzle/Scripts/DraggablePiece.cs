using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggablePiece : MonoBehaviour
{
    public enum DraggablePieceIndex
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
    public DraggablePieceIndex draggablePieceIndex;
    [HideInInspector]
    public bool foundFinalPiece = false;
    [HideInInspector]
    public FinalPiece finalPieceInstance = null;

    // Private variables
    private bool isDragging = false;
    private Vector3 localStartPosition;

    // Start is called before the first frame update
    void Start()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        circleCollider.enabled = false;

        spriteRenderer = GetComponent<SpriteRenderer>();

        localStartPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDragging)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            transform.Translate(mousePos);
        }
    }

    /// <summary
    /// To reset the game
    /// </summary>

    public void ResetToStartPosition()
    {
        transform.localPosition = localStartPosition;
    }

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

    /// <summary>
    /// Mouse Interaction
    /// </summary>

    private void OnMouseDown()
    {
        isDragging = true;
    }

    private void OnMouseUp()
    {
        isDragging = false;

        // If found final piece then disable draggable piece
        if (foundFinalPiece && finalPieceInstance != null)
        {
            // Hide the draggable piece and disable its collider
            DisableSpriteRenderer();
            DisableCollider();

            // Show the final piece and disable its collider
            finalPieceInstance.EnableSpriteRenderer();
            finalPieceInstance.DisableCollider();

            // Add solved piece to jigsaw puzzle script
            JigsawPuzzle.instance.AddSolvedPiece();
        }
        else
        {
            // Reset the draggable piece to its start position
            ResetToStartPosition();
        }
    }
}
