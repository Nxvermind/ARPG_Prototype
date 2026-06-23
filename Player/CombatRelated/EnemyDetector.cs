using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class EnemyDetector : MonoBehaviour
{
    public HashSet<Transform> EnemiesDetected { get; private set; } = new();

    public int NumOfEnemiesDetected => EnemiesDetected.Count;

    [SerializeField] private LayerMask enemyLayer;

    public static event Action OnEnemyDetection;
    public static event Action OnEnemyCleared;


    private void OnEnable()
    {
        EventBus.OnEnemyDeathEvent += OnEnemyDeath;
    }

    private void OnDisable()
    {
        EventBus.OnEnemyDeathEvent -= OnEnemyDeath;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemiesDetected.Add(other.transform);

            if (NumOfEnemiesDetected == 1) OnEnemyDetection?.Invoke();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (other.TryGetComponent<Transform>(out var enemy))
            {
                EnemiesDetected.Remove(enemy);
            }

            if (NumOfEnemiesDetected == 0) OnEnemyCleared?.Invoke();
        }
    }

    private void OnEnemyDeath(Enemy enemy)
    {
        EnemiesDetected.Remove(enemy.body);

        if(NumOfEnemiesDetected == 0) OnEnemyCleared?.Invoke();
    }
}
