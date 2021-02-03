using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BoxTrigger : MonoBehaviour
{
    private bool checkForInput = false;
    private SpriteRenderer parentSR;
    private SpriteRenderer sR;

    void Start()
    {
        parentSR = transform.parent.gameObject.GetComponent<SpriteRenderer>();
        sR = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            sR.enabled = true;
            checkForInput = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            sR.enabled = false;
            checkForInput = false;
        }
    }

    void Update()
    {
        if (checkForInput)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (Player.instance.axeDrawn)
                {
                    parentSR.sprite = GameAssets.instance.treasureBoxClosedSprite;
                    Player.instance.axeDrawn = false;
                    Player.UnequipAxe();
                }
                else
                {
                    parentSR.sprite = GameAssets.instance.treasureBoxOpenSprite;
                    Player.instance.axeDrawn = true;
                    Player.EquipAxe();
                }
            }
        }
    }
}
