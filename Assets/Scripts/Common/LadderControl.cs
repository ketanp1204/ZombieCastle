﻿using System.Collections;
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

    private bool climbingLadder = false;

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
        if (collision.CompareTag("Player") && collision.gameObject.layer != 12)
        {
            triggerStay = true;

            playerGameObject = collision.gameObject;

            playerAnimator = playerGameObject.GetComponent<Animator>();

            if (triggerLocation == TriggerLocation.Top)
            {
                popupTextUI.text = "F - Climb Down Ladder";
            }
            else
            {
                popupTextUI.text = "F - Climb Up Ladder";
            }
            
            new Task(UIAnimation.FadeTMProTextAfterDelay(popupTextUI, 0f, 1f, 0f, 0.1f));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            triggerStay = false;

            if (!climbingLadder)
            {
                new Task(UIAnimation.FadeTMProTextAfterDelay(popupTextUI, 1f, 0f, 0f, 0.1f));
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && triggerStay)
        {
            ladderBoxCollider.enabled = false;
            Player.StopMovement();
            Player.DisableAttackInput();

            new Task(UIAnimation.FadeTMProTextAfterDelay(popupTextUI, 1f, 0f, 0f, 0.1f));

            if (triggerLocation == TriggerLocation.Top)
            {
                playerAnimator.SetBool("LadderClimbDown", true);

                Rigidbody2D rb = playerGameObject.GetComponent<Rigidbody2D>();
                StartCoroutine(MovePlayerRigidBody(triggerLocation, rb, rb.position.y, rb.position.y - 10.5f, 0f));
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
            climbingLadder = true;

            float _timeStartedLerping = Time.time;
            float timeSinceStarted;
            float percentageComplete;

            Player.instance.SetClimbingLadderDown();

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
                    Player.instance.UnsetClimbingLadderDown();
                    new Task(EnablePlayerInputAfterDelay());
                    climbingLadder = false;
                    break;
                }

                yield return new WaitForEndOfFrame();
            }
        }
        else if (triggerLocation == TriggerLocation.Bottom)
        {
            climbingLadder = true;

            float _timeStartedLerping = Time.time;
            float timeSinceStarted;
            float percentageComplete;

            Player.instance.SetClimbingLadderUp();

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
                    Player.instance.UnsetClimbingLadderUp();
                    new Task(EnablePlayerInputAfterDelay());
                    climbingLadder = false;
                    break;
                }

                yield return new WaitForEndOfFrame();
            }
        }
    }

    private IEnumerator EnablePlayerInputAfterDelay()
    {
        yield return new WaitForSeconds(0.1f);

        Player.EnableMovement();
        Player.EnableAttackInputAfterDelay();
    }
}
