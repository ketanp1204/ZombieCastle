﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class R2_Combat : MonoBehaviour
{
    // Singleton
    public static R2_Combat instance;

    // Public References
    public GameObject zombieGameObjectsContainer;

    public CinemachineVirtualCamera cinemachineCamera;

    // Private References
    private List<GameObject> zombieGameObjects;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        zombieGameObjects = new List<GameObject>();
        foreach (Transform child in zombieGameObjectsContainer.transform)
        {
            zombieGameObjects.Add(child.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Disable this collider
        GetComponent<BoxCollider2D>().enabled = false;

        // Set player camera to combat camera position
        Player.instance.UpdateCombatCameraPosition(zombieGameObjects[0].transform);
        cinemachineCamera.Follow = Player.instance.combatCameraPosition;

        // Set player combat status
        Player.SetCombatState();

        // Start chasing by the zombies after knife selected by player in the inventory box
        foreach (GameObject gameObject in zombieGameObjects)
        {
            gameObject.GetComponent<EnemyAI>().StartChasingPlayer();
        }
    }
}
