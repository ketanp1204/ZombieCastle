
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public static PlayerInput instance;

    public float horizontalInput { get; private set; }
    public float verticalInput { get; private set; }
    public bool walkFastInput { get; private set; }
    public bool jumpInput { get; private set; }
    public bool leftMousePressed { get; private set; }
    public bool attack2Pressed { get; private set; }
    public bool attack1Released { get; private set; }
    public bool attack2Released { get; private set; }
    public bool interactKey { get; private set; }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        ResetLeftMouseDownBool();

        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            walkFastInput = true;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            walkFastInput = false;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpInput = true;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            jumpInput = false;
        }

        if (Input.GetMouseButtonDown(0))
        {
            leftMousePressed = true;
        }

        if (Input.GetMouseButtonDown(1))
        {
            attack2Pressed = true;
        }

        if(Input.GetMouseButtonUp(0))
        {
            attack1Released = true;
            leftMousePressed = false;
        }

        if(Input.GetMouseButtonUp(1))
        {
            attack2Released = true;
            attack2Pressed = false;
        }

        if(Input.GetKeyDown(KeyCode.E))
        {
            interactKey = true;
        }

        if(Input.GetKeyUp(KeyCode.E))
        {
            interactKey = false;
        }
    }

    private void ResetLeftMouseDownBool()
    {
        leftMousePressed = false;
    }
}
