using UnityEngine;

public class StairsTwoDCollider : MonoBehaviour
{
    private float playerWalkSpeed;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();
        playerWalkSpeed = player.wSpeed;
        player.wSpeed = player.rSpeed;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();
        player.wSpeed = playerWalkSpeed;
    }
}
