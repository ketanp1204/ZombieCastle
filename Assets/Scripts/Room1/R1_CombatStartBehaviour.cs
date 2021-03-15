using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class R1_CombatStartBehaviour : MonoBehaviour
{
    // Public References
    public GameObject zombieGameObjectsContainer;

    // Private References
    private List<GameObject> zombieGameObjects;

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
        Debug.Log("zombies start chasing");

        // Disable this collider
        GetComponent<BoxCollider2D>().enabled = false;

        // Show player dialogue about using knife

        // Open inventory box

        // After closing inventory box, start chasing by the zombies
        foreach (GameObject gameObject in zombieGameObjects)
        {
            gameObject.GetComponent<EnemyAI>().StartChasingPlayer();
        }
    }


}
