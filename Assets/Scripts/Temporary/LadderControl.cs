using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LadderControl : MonoBehaviour
{
    public enum TriggerLocation
    {
        Top,
        Bottom
    }

    // Public 
    public TriggerLocation triggerLocation;

    // Private
    private UIReferences uiReferences;

    private BoxCollider2D ladderBoxCollider;

    private TextMeshProUGUI ladderUseInstruction;

    private bool triggerStay = false;

    private GameObject playerGameObject;

    private void Start()
    {
        uiReferences = GameSession.instance.uiReferences;
        ladderUseInstruction = uiReferences.ladderUseInstruction;

        ladderBoxCollider = transform.parent.GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            triggerStay = true;

            playerGameObject = collision.gameObject;

            new Task(UIAnimation.FadeTMProTextAfterDelay(ladderUseInstruction, 0f, 1f, 0f, 0.1f));

            if (triggerLocation == TriggerLocation.Top)
            {
                
            }
            else
            {

            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            triggerStay = false;

            new Task(UIAnimation.FadeTMProTextAfterDelay(ladderUseInstruction, 1f, 0f, 0f, 0.1f));
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && triggerStay)
        {
            ladderBoxCollider.enabled = false;
            Player.StopMovement();

            if (triggerLocation == TriggerLocation.Top)
            {
                Rigidbody2D rb = playerGameObject.GetComponent<Rigidbody2D>();
                StartCoroutine(MovePlayerRigidBody(rb, rb.position.y, rb.position.y - 10.5f, 0f));
            }
            else
            {
                Rigidbody2D rb = playerGameObject.GetComponent<Rigidbody2D>();
                StartCoroutine(MovePlayerRigidBody(rb, rb.position.y, rb.position.y + 10.5f, 0f));
            }
        }
    }

    private IEnumerator MovePlayerRigidBody(Rigidbody2D rb, float startY, float endY, float delay, float lerpTime = 1f)
    {
        yield return new WaitForSeconds(delay);

        float _timeStartedLerping = Time.time;
        float timeSinceStarted;
        float percentageComplete;

        while (true)
        {
            timeSinceStarted = Time.time - _timeStartedLerping;
            percentageComplete = timeSinceStarted / lerpTime;

            float currentValue = Mathf.Lerp(startY, endY, percentageComplete);

            if (rb != null)
            {
                rb.MovePosition(new Vector2(rb.position.x, currentValue));
            }

            if (percentageComplete >= 1)
            {
                break;
            }

            yield return new WaitForEndOfFrame();
        }

        if (triggerLocation == TriggerLocation.Bottom)
        {
            lerpTime = 0.5f;
            _timeStartedLerping = Time.time;

            float startX = rb.position.x;
            float endX = rb.position.x - 2f;

            while (true)
            {
                timeSinceStarted = Time.time - _timeStartedLerping;
                percentageComplete = timeSinceStarted / lerpTime;

                float currentValue = Mathf.Lerp(startX, endX, percentageComplete);

                if (rb != null)
                {
                    rb.MovePosition(new Vector2(currentValue, rb.position.y));
                }

                if (percentageComplete >= 1)
                {
                    ladderBoxCollider.enabled = true;
                    Player.EnableMovement();
                    break;
                }

                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            Player.EnableMovement();
        }
    }
}
