using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class R1_CombatStartBehaviour : MonoBehaviour
{
    // Singleton
    public static R1_CombatStartBehaviour instance;

    // Public References
    public GameObject zombieGameObjectsContainer;
    public CinemachineVirtualCamera cinemachineCamera;
    public Transform temporaryCameraTarget;
    public GameObject combatBlockRightWall;
    public WeaponObject knifeScriptableObject;

    // Private References
    private List<GameObject> zombieGameObjects;

    // Private variables
    [TextArea(4, 10)]
    public string[] playerResponseOnFindingZombies = { "" };

    private float dialogueDisplayDelay = 2f;

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
        combatBlockRightWall.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Disable this collider
        GetComponent<BoxCollider2D>().enabled = false;

        // Move camera to show zombie
        Camera.main.gameObject.GetComponent<CinemachineBrain>().m_IgnoreTimeScale = true;
        cinemachineCamera.Follow = temporaryCameraTarget;

        // Show player dialogue about using knife
        if (DialogueBox.instance)
        {
            StartCoroutine(ChangeCameraTargetAfterDelay(dialogueDisplayDelay));
            DialogueBox.instance.dialogueDisplayDelay = dialogueDisplayDelay;
            DialogueBox.instance.SetRoom1ZombieDiscoveryFlag();
            DialogueBox.instance.FillSentences(playerResponseOnFindingZombies);
            DialogueBox.instance.StartDialogueDisplay();
        }
    }

    public void ShowInventoryAfterKnifeOpenDialogue()
    {
        StartCoroutine(WaitForTimeScaleToReset());

        // Open inventory box and highlight knife
        InventoryManager.ShowInventory();
        InventoryManager.instance.HighlightWeaponBeforeCombatStart(knifeScriptableObject);
    }

    private IEnumerator ChangeCameraTargetAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Move camera back to player
        cinemachineCamera.Follow = Player.instance.gameObject.transform;

        yield return new WaitForSeconds(1f);

        Camera.main.gameObject.GetComponent<CinemachineBrain>().m_IgnoreTimeScale = false;
    }

    private IEnumerator WaitForTimeScaleToReset()
    {
        yield return new WaitForSeconds(0.1f);

        // Block the player in the combat area
        combatBlockRightWall.SetActive(true);

        // Start chasing by the zombies after knife selected by player in the inventory box
        foreach (GameObject gameObject in zombieGameObjects)
        {
            gameObject.GetComponent<EnemyAI>().StartChasingPlayer();
        }

        Player.SetCombatState();

        // Set player camera to combat camera position
        Player.instance.UpdateCombatCameraPosition(zombieGameObjects[0].transform);
        cinemachineCamera.Follow = Player.instance.combatCameraPosition;
    }
}
