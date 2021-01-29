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
                if (Player.instance.weaponDrawn)
                {
                    parentSR.sprite = GameAssets.instance.treasureBoxClosedSprite;
                    Player.instance.weaponDrawn = false;
                    Player.RemoveWeapon();
                }
                else
                {
                    parentSR.sprite = GameAssets.instance.treasureBoxOpenSprite;
                    Player.instance.weaponDrawn = true;
                    Player.DrawWeapon();
                }
            }
        }
    }
}
