using System.Collections.Generic;
using UnityEngine;

public class ExecutionManager : MonoBehaviour
{
    [SerializeField] private Transform player;

    public Enemy currentExecutableEnemy;

    private List<Enemy> enemyList = new List<Enemy>();

    private void OnEnable()
    {
        BasicEnemy.OnExecutionRequest += AddEnemy;

        EventBus.OnEnemyDeathEvent += RemoveEnemy;
    }

    private void OnDisable()
    {
        BasicEnemy.OnExecutionRequest -= AddEnemy;

        EventBus.OnEnemyDeathEvent += RemoveEnemy;
    }
    private void Update()
    {
        if (enemyList.Count == 0)
        {
            //Debug.Log("there is no enemy");
            currentExecutableEnemy = null;
            return;
        }

        currentExecutableEnemy = FindClosestEnemyForExecution(player);
    }

    private void AddEnemy(Enemy enemy)
    {
        if (!enemyList.Contains(enemy))
        {
            enemyList.Add(enemy);

        }
    }

    public void RemoveEnemy(Enemy enemy)
    {
        enemyList.Remove(enemy);
    }

    private Enemy FindClosestEnemyForExecution(Transform player)
    {
        Enemy closestEnemy = null;

        float closestDistanceSqr = Mathf.Infinity;

        foreach (Enemy enemy in enemyList)
        {
            Vector3 directionToEnemy = enemy.transform.position - player.position;

            float sqrDistance = directionToEnemy.sqrMagnitude;

            if (sqrDistance < closestDistanceSqr)
            {
                closestDistanceSqr = sqrDistance;
                closestEnemy = enemy;
            }

        }

        return closestEnemy;
    }
}
