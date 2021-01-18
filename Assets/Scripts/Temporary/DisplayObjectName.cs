using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayObjectName : MonoBehaviour
{
    private Canvas dynamicUICanvas;
    private Camera mainCamera;
    private GameObject objectNameGO;

    private Vector3 screenPosition;
    private string objectName;

    // Start is called before the first frame update
    void Start()
    {
        dynamicUICanvas = GameSession.instance.dynamicUICanvas;
        mainCamera = GameSession.instance.mainCamera;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Object"))
        {
            // Get name of newly found object
            objectName = GetComponent<ObjectProperties>().objectName;

            // Get reference to current ObjectName gameObject
            objectNameGO = GameSession.instance.objectNameGO;

            // Show interaction key
            if (objectNameGO != null)
            {
                Destroy(objectNameGO);
            }

            // Instantiate ObjectName prefab
            objectNameGO = Instantiate(GameAssets.instance.objectNamePrefab, dynamicUICanvas.transform);

            // Set ObjectName text
            objectNameGO.GetComponentInChildren<TextMeshProUGUI>().text = objectName;

            // Set object name box position to slightly above the current object
            screenPosition = mainCamera.WorldToScreenPoint(collision.transform.position);
            screenPosition.y += 100f;
            objectNameGO.transform.position = screenPosition;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Object"))
        {
            if (objectNameGO != null)
            {
                // Update box position
                screenPosition = mainCamera.WorldToScreenPoint(collision.transform.position);
                screenPosition.y += 100f;
                objectNameGO.transform.position = screenPosition;
            }
            else
            {
                // Instantiate ObjectName prefab
                objectNameGO = Instantiate(GameAssets.instance.objectNamePrefab, dynamicUICanvas.transform);

                // Set ObjectName text
                objectNameGO.GetComponentInChildren<TextMeshProUGUI>().text = objectName;
            }

            if (PlayerInput.instance.interactKey)
            {
                // Handle interaction events
                Debug.Log("interact");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Object"))
        {
            Destroy(objectNameGO);
            /*
            if (objectName == objectNameGO.GetComponentInChildren<TextMeshProUGUI>().text)
            {
                Destroy(objectNameGO);
            }
            */
        }
    }
}
