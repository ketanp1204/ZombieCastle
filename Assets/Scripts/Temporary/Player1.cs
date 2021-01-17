using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController2D))]
[RequireComponent(typeof(PlayerInput))]
public class Player1 : MonoBehaviour
{
    // Private Variables
    private float gravity = -20f;
    private float jumpVelocity = 8f;
    private Vector3 velocity;
    private float velocityXSmoothing;
    private float accelerationTimeAirborne = 0.2f;
    private float accelerationTimeGrounded = 0.1f;

    // Public Variables
    public float walkSpeed = 7f;
    public float ladderMoveSpeed = 2f;
    public float jumpHeight = 2f;
    public float timeToJumpApex = 0.3f;
    [HideInInspector]
    public bool onLadder = false;

    // Component References
    private CharacterController2D controller;
    private PlayerInput playerInput;

    private void Start()
    {
        controller = GetComponent<CharacterController2D>();
        playerInput = GetComponent<PlayerInput>();

        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
    }

    void Update()
    {
        if (!onLadder && (controller.collisions.above || controller.collisions.below))
        {
            velocity.y = 0f;
        }

        Vector2 input = new Vector2(playerInput.horizontalInput, playerInput.verticalInput);

        if (playerInput.jumpInput && controller.collisions.below)
        {
            velocity.y = jumpVelocity;
        }

        float targetVelocityX = input.x * walkSpeed;
        velocity.x = Mathf.SmoothDamp(  velocity.x, 
                                        targetVelocityX, 
                                        ref velocityXSmoothing, 
                                        (controller.collisions.below)?accelerationTimeGrounded:accelerationTimeAirborne);

        if (!onLadder)
            velocity.y += gravity * Time.deltaTime;
        else
            velocity.y += input.y * ladderMoveSpeed;
        
        controller.Move(velocity * Time.deltaTime, onLadder);
    }
}
