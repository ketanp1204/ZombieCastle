using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class R5_TopFloorCombatStart : MonoBehaviour
{
    // Public References
    public GameObject zombieGameObjectsContainer;
    public CinemachineVirtualCamera cinemachineCamera;
    public GameObject combatBlockWall1;
    public GameObject combatBlockWall2;
    public GameObject zombie3PathfindingObject;
    public GameObject bossPathfindingObject;

    // Private References
    private List<GameObject> zombieGameObjects;
    private BoxCollider2D combatStartCollider;

    // Start is called before the first frame update
    void Start()
    {
        combatBlockWall1.SetActive(false);
        combatBlockWall2.SetActive(false);
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
            zombie3PathfindingObject.SetActive(false);
            bossPathfindingObject.SetActive(true);
        }
        else
        {
            combatStartCollider.enabled = true;
            zombie3PathfindingObject.SetActive(true);
            bossPathfindingObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Block the player in the combat area
            combatBlockWall1.SetActive(true);
            combatBlockWall2.SetActive(true);

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
