using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Cached References
    private CharacterController2D characterController;
    private PlayerInput playerInput;

    // Variables
    public float walkSpeed;
    public float runSpeed;
    float horizontal;
    float vertical;


    // Start is called before the first frame update
    void Awake()
    {
        characterController = GetComponent<CharacterController2D>();
        playerInput = GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // characterController.Move(playerInput.horizontalInput * walkSpeed * Time.fixedDeltaTime);
    }
}
