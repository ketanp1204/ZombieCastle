
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public float horizontalInput { get; private set; }
    public float verticalInput { get; private set; }
    public bool runInput { get; private set; }
    public bool attack1Pressed { get; private set; }
    public bool attack2Pressed { get; private set; }
    public bool attack1Released { get; private set; }
    public bool attack2Released { get; private set; }

    private void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            runInput = true;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            runInput = false;
        }

        if (Input.GetMouseButtonDown(0))
        {
            attack1Pressed = true;
        }

        if (Input.GetMouseButtonDown(1))
        {
            attack2Pressed = true;
        }

        if(Input.GetMouseButtonUp(0))
        {
            attack1Released = true;
            attack1Pressed = false;
        }

        if(Input.GetMouseButtonUp(1))
        {
            attack2Released = true;
            attack2Pressed = false;
        }
    }
}
