using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairsReblockCollider : MonoBehaviour
{
    private BoxCollider2D col;

    public BoxCollider2D stairsBlockCollider;
    public GameObject stairsBlockDisplayGO;

    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        col.enabled = false;

        stairsBlockCollider.enabled = true;

        stairsBlockDisplayGO.SetActive(true);
    }
}
