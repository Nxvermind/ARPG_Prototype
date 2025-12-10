using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class EnemyDetector : MonoBehaviour
{
    public HashSet<Transform> enemiesDetected = new();

    public int NumOfEnemiesDetected;

    public float interval = 0.2f;
    public float timer;

    [Header("To Find Enemies")]
    public float visionAngle;

    public float visionRange;

    public Transform CurrentTarget { get; private set; }

    private void Start()
    {
        timer = interval;
    }

    private void OnEnable()
    {
        EventBus.OnEnemyDeathEvent += OnEnemyDeath;        
    }

    private void OnDisable()
    {
        EventBus.OnEnemyDeathEvent -= OnEnemyDeath;     
    }

    void Update()
    {
        NumOfEnemiesDetected = enemiesDetected.Count;

        if (NumOfEnemiesDetected == 0)
        {
            CurrentTarget = null;

            return;
        }

        if (SwitchCameras.IsLockOnTargetCameraActive)
        {
            CurrentTarget = LockOnTargetCamera.CurrentLockOnTarget;
            return;
        }

        timer -= Time.deltaTime;

        if(timer <= 0)
        {
            timer = interval;
            CurrentTarget = FindClosestEnemy(transform);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Transform enemy = other.GetComponent<Transform>();

            enemiesDetected.Add(enemy);

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Transform enemy = other.GetComponent<Transform>();

            enemiesDetected.Remove(enemy);
        }
    }

    public void RotateToEnemy()
    {
        if (CurrentTarget == null) return;

        Vector3 direction = (CurrentTarget.position - transform.parent.position).normalized;
        direction.y = 0;

        transform.parent.rotation = Quaternion.LookRotation(direction);
    }

    public bool IsCurrentTargetClose(float distanceToEnemy)
    {
        if(CurrentTarget == null)  return false; 

        Vector3 distance = CurrentTarget.position - transform.position;

        float sqrDistance = distance.sqrMagnitude;

        float sqrDistanceToPlayer = distanceToEnemy * distanceToEnemy;

        return sqrDistance <= sqrDistanceToPlayer;
    }

    public Transform FindClosestEnemy(Transform player)
    {
        Transform closestEnemy = null;

        float closestDistanceSqr = Mathf.Infinity;

        foreach (Transform enemy in enemiesDetected)
        {
            Vector3 directionToEnemy = enemy.position - player.position;

            float sqrDistance = directionToEnemy.sqrMagnitude;

            if (sqrDistance < closestDistanceSqr)
            {
                closestDistanceSqr = sqrDistance;
                closestEnemy = enemy;
            }

        }

        return closestEnemy;
    }

    public Transform FindClosestEnemyInVisionAngle(Transform player, float visionAngle)
    {
        Transform closestEnemy = null;
        float closestDistanceSqr = Mathf.Infinity;

        foreach (Transform enemy in enemiesDetected)
        {
            Vector3 directionToEnemy = enemy.position - player.position;
            float angle = Vector3.Angle(player.forward, directionToEnemy.normalized);

            if (angle > visionAngle)
            {
                //Debug.Log("the angle is above visionAngle");
                continue;
            }

            float distanceSqr = directionToEnemy.sqrMagnitude;
            if (distanceSqr < closestDistanceSqr)
            {
                closestDistanceSqr = distanceSqr;
                closestEnemy = enemy;
            }
        }
        return closestEnemy;
    }

    public Transform FindClosestEnemyInVisionAngleFromPosition(Vector3 origin, Vector3 forward, float angle)
    {
        Transform closestEnemy = null;
        float minDistance = Mathf.Infinity;

        foreach (var enemy in enemiesDetected)
        {
            if (enemy == null) continue;

            Vector3 dirToEnemy = (enemy.position - origin).normalized;
            float distToEnemy = Vector3.Distance(origin, enemy.position);
            float angleToEnemy = Vector3.Angle(forward, dirToEnemy);

            if (angleToEnemy <= angle)
            {
                if (distToEnemy < minDistance)
                {
                    minDistance = distToEnemy;
                    closestEnemy = enemy;
                }
            }
        }

        return closestEnemy;
    }

    private void OnEnemyDeath(Enemy enemy)
    {
        enemiesDetected.Remove(enemy.transform);

        CurrentTarget = null;

        NumOfEnemiesDetected = enemiesDetected.Count;
    }

}
