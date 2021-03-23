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

    private TextMeshProUGUI popupTextUI;

    private bool triggerStay = false;

    private GameObject playerGameObject;

    private Animator playerAnimator;

    private void Start()
    {
        uiReferences = GameSession.instance.uiReferences;
        popupTextUI = uiReferences.popupTextUI;

        ladderBoxCollider = transform.parent.GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            triggerStay = true;

            playerGameObject = collision.gameObject;

            playerAnimator = playerGameObject.GetComponent<Animator>();

            popupTextUI.text = "F - Use Ladder";
            new Task(UIAnimation.FadeTMProTextAfterDelay(popupTextUI, 0f, 1f, 0f, 0.1f));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            triggerStay = false;

            new Task(UIAnimation.FadeTMProTextAfterDelay(popupTextUI, 1f, 0f, 0f, 0.1f));
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && triggerStay)
        {
            ladderBoxCollider.enabled = false;
            Player.StopMovement();

            new Task(UIAnimation.FadeTMProTextAfterDelay(popupTextUI, 1f, 0f, 0f, 0.1f));

            if (triggerLocation == TriggerLocation.Top)
            {
                playerAnimator.SetBool("LadderClimbDown", true);

                Rigidbody2D rb = playerGameObject.GetComponent<Rigidbody2D>();
                StartCoroutine(MovePlayerRigidBody(triggerLocation, rb, rb.position.y - 1.5f, rb.position.y - 10.5f, 0f));
            }
            else
            {
                playerAnimator.SetBool("LadderClimbUp", true);

                Rigidbody2D rb = playerGameObject.GetComponent<Rigidbody2D>();
                StartCoroutine(MovePlayerRigidBody(triggerLocation, rb, rb.position.y, rb.position.y + 10.5f, 0f));
            }
        }
    }

    private IEnumerator MovePlayerRigidBody(TriggerLocation triggerLocation, Rigidbody2D rb, float startY, float endY, float delay, float lerpTime = 3f)
    {
        yield return new WaitForSeconds(delay);


        if (triggerLocation == TriggerLocation.Top)
        {
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
                    playerAnimator.SetBool("LadderClimbDown", false);
                    Player.EnableMovement();
                    break;
                }

                yield return new WaitForEndOfFrame();
            }
        }
        else if (triggerLocation == TriggerLocation.Bottom)
        {
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
                    ladderBoxCollider.enabled = true;
                    playerAnimator.SetBool("LadderClimbUp", false);
                    Player.EnableMovement();
                    break;
                }

                yield return new WaitForEndOfFrame();
            }

            /*
            lerpTime = 0.05f;
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
            */
        }
    }
}
