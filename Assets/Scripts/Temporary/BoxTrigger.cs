﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BoxTrigger : MonoBehaviour
{
    private bool checkForInput = false;
    private SpriteRenderer sR;

    void Start()
    {
        sR = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        sR.enabled = true;
        checkForInput = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        sR.enabled = false;
        checkForInput = false;
    }

    void Update()
    {
        if (checkForInput)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                // Handle box open events
            }
        }
    }
}
