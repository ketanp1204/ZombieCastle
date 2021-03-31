using System.Collections;
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
    public GameObject combatBlockWall;
    public GameObject leftWallCollider;
    public BoxCollider2D treasureBoxCollider;

    // Private References
    private List<GameObject> zombieGameObjects;
    private BoxCollider2D combatStartCollider;

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
        combatStartCollider = GetComponent<BoxCollider2D>();
        zombieGameObjects = new List<GameObject>();
        foreach (Transform child in zombieGameObjectsContainer.transform)
        {
            zombieGameObjects.Add(child.gameObject);
        }
        combatBlockWall.SetActive(false);

        if (GameData.r2_combatCompleted)
        {
            foreach (GameObject zombie in zombieGameObjects)
            {
                Destroy(zombie);
            }
            combatStartCollider.enabled = false;
            leftWallCollider.SetActive(true);

            if (!GameData.r2_treasureBoxMagicPotionCollected)
            {
                treasureBoxCollider.enabled = true;
            }
            else
            {
                treasureBoxCollider.enabled = false;
            }
        }
        else
        {
            combatStartCollider.enabled = true;
            leftWallCollider.SetActive(false);
            treasureBoxCollider.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Disable this collider
        combatStartCollider.enabled = false;

        // Set player camera to combat camera position
        Player.instance.UpdateCombatCameraPosition(zombieGameObjects[0].transform);
        cinemachineCamera.Follow = Player.instance.combatCameraPosition;

        // Set player combat status
        Player.SetCombatState();

        // Block the player in the combat area
        combatBlockWall.SetActive(true);

        // Start chasing by the zombies
        foreach (GameObject gameObject in zombieGameObjects)
        {
            gameObject.GetComponent<EnemyAI>().StartChasingPlayer();
        }
    }
}
