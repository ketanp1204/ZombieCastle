using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class R5_TopFloorCombatStart : MonoBehaviour
{
    // Public References
    public GameObject zombieGameObjectsContainer;
    public CinemachineVirtualCamera cinemachineCamera;
    public GameObject combatBlockWall;

    // Private References
    private List<GameObject> zombieGameObjects;
    private BoxCollider2D combatStartCollider;

    // Start is called before the first frame update
    void Start()
    {
        combatBlockWall.SetActive(false);
        zombieGameObjects = new List<GameObject>();
        foreach (Transform child in zombieGameObjectsContainer.transform)
        {
            zombieGameObjects.Add(child.gameObject);
        }

        combatStartCollider = GetComponent<BoxCollider2D>();

        // Check if combat already completed
        if (GameData.r5_zombie3CombatCompleted)
        {
            foreach (GameObject zombie in zombieGameObjects)
            {
                Destroy(zombie);
            }
            combatStartCollider.enabled = false;
        }
        else
        {
            combatStartCollider.enabled = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Block the player in the combat area
            combatBlockWall.SetActive(true);

            // Disable this collider
            combatStartCollider.enabled = false;

            // Set player camera to combat camera position
            Player.instance.UpdateCombatCameraPosition(zombieGameObjects[0].transform);
            cinemachineCamera.Follow = Player.instance.combatCameraPosition;

            // Set player combat status
            Player.SetCombatState();

            // Start chasing by the zombies after knife selected by player in the inventory box
            foreach (GameObject gameObject in zombieGameObjects)
            {
                gameObject.GetComponent<RangedEnemyAI>().StartChasingPlayer();
            }
        }
    }
}
