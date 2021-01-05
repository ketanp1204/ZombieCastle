using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderControl : MonoBehaviour
{

    Vector3 pointOfContact;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            pointOfContact = collision.GetComponent<Collider2D>().bounds.ClosestPoint(transform.position);

            collision.GetComponent<Rigidbody2D>().gravityScale = 0f;
            collision.GetComponent<Player>().isClimbingLadder = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.GetComponent<Rigidbody2D>().gravityScale = collision.GetComponent<Player>().gravityScale;
            collision.GetComponent<Player>().isClimbingLadder = false;
        }
    }
}
