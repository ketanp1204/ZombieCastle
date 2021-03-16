using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MultipleZombiesDeathBehaviour : MonoBehaviour
{
    // Singleton
    public static MultipleZombiesDeathBehaviour instance;

    public UnityEvent allZombiesDead;

    private int numberOfZombies;
    private int currentNumberOfDeadZombies = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        numberOfZombies = transform.childCount;
    }

    public void AddDeadZombie()
    {
        currentNumberOfDeadZombies += 1;

        if (currentNumberOfDeadZombies == numberOfZombies)
        {
            allZombiesDead.Invoke();
        }
    }

}
