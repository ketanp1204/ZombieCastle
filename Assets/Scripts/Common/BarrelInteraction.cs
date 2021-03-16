using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelInteraction : MonoBehaviour
{
    // Singleton
    public static BarrelInteraction instance;

    // Private References
    private BoxCollider2D boxCollider;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void EnableCollider()
    {
        boxCollider.enabled = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
