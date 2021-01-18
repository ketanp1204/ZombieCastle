using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerObjectSelection : MonoBehaviour
{
    [HideInInspector]
    public Canvas dynamicUICanvas;
    [HideInInspector]
    public Camera mainCamera;

    private GameObject objectNameGO;
    private Vector3 screenPosition;
    private bool nameBoxReplaced = false;
    private string previousObjectName;

    // Start is called before the first frame update
    void Start()
    {
        dynamicUICanvas = GameSession.instance.dynamicUICanvas;
        mainCamera = GameSession.instance.mainCamera;
    }

    // TODO: BUG FIX REQUIRED - Entering multiple colliders changes the name of the current object.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Object"))
        {
            // Get name of newly found object
            string objectName = collision.GetComponent<ObjectProperties>().objectName;

            // Show interaction key
            if (objectNameGO != null)
            {
                if (!previousObjectName.Equals(objectName))
                {
                    previousObjectName = objectNameGO.GetComponentInChildren<TextMeshProUGUI>().text;
                    Destroy(objectNameGO);
                    nameBoxReplaced = true;
                }
            }
            else
            {
                previousObjectName = "";
            }

            // Instantiate ObjectName prefab
            objectNameGO = Instantiate(GameAssets.instance.objectNamePrefab, dynamicUICanvas.transform);

            // Set ObjectName text
            objectNameGO.GetComponentInChildren<TextMeshProUGUI>().text = objectName;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Object"))
        {
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
            if (!nameBoxReplaced)
            {
                Destroy(objectNameGO);
            }
            else
            {
                nameBoxReplaced = false;
                if (previousObjectName != "")
                {
                    objectNameGO.GetComponentInChildren<TextMeshProUGUI>().text = previousObjectName;
                }
            }
        }
    }
}
