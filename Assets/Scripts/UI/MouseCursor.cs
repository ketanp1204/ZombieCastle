using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseCursor : MonoBehaviour
{
    public float offsetX;
    public float offsetY;

    private Image cursorImage;

    // Start is called before the first frame update
    void Start()
    {
        cursorImage = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Cursor.lockState == CursorLockMode.None)
        {
            cursorImage.enabled = true;
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.x += offsetX;
            mousePosition.y += offsetY;
            transform.position = mousePosition;
        }  
        else
        {
            cursorImage.enabled = false;
        }
    }
}
