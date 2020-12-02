using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class StairsBlockedArea : MonoBehaviour
{
    [SerializeField]
    List<Enemy> enemies;

    [SerializeField]
    UnityEvent OnEnemiesKilled;

    List<Enemy> enemiesRemaining;

    public CameraFollow cameraFollow;

    void OnEnable()
    {
        enemiesRemaining = new List<Enemy>(enemies);
        
        foreach(Enemy enemy in enemiesRemaining)
        {
            enemy.OnDeath += HandleEnemyDeath;                          // Registering for OnDeath event on Enemy
        }

        SetCameraBounds(-2.8f, 2.82f, -2.83f, -1.99f);
    }

    void HandleEnemyDeath(Enemy enemy)
    {
        enemiesRemaining.Remove(enemy);

        if(enemiesRemaining.Count == 0)
        {
            SetCameraBounds(-2.8f, 2.82f, -2.83f, 2.81f);
            OnEnemiesKilled.Invoke();
        }
    }

    public void SetCameraBounds(float xMin, float xMax, float yMin, float yMax)
    {
        cameraFollow.SetCameraBounds(xMin, xMax, yMin, yMax);     // TODO: Remove hard coded values
    }

    [ContextMenu("Autofill Zombie Enemies")]
    void AutoFillZombies()
    {
        enemies = FindObjectsOfType<Enemy>()
            .Where(t => t.name.ToLower().Contains("zombie"))
            .ToList();
    }
}
