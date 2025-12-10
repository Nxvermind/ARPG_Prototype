using System.Collections.Generic;
using UnityEngine;

public static class EnemyDetection
{
    //detectar enemigos en un area circular
    //el hashSet lee a los enemigos que están almacenados en el collider
    public static void DetectEnemiesInRadius(Vector3 origin, float radius, LayerMask enemyMask, Collider[] enemyStorage, HashSet<Transform> enemiesSet)
    {
        //detecto a los enemigos
        int enemiesDetected = Physics.OverlapSphereNonAlloc(origin, radius, enemyStorage, enemyMask);

        //elimina del collider a los enemigos que ya no son detectados
        for(int i = enemiesDetected; i < enemyStorage.Length; i++)
        {
            enemyStorage[i] = null;
        }

        //si el enemigo es destruido (o sea que lo maté) entonces lo quita
        enemiesSet.RemoveWhere(enemy  => enemy == null);

        //si el enemigo sale del radio del overlapSphere entonces tambien lo quita
        enemiesSet.RemoveWhere(enemy =>
        {
            bool stillInRange = false;

            for (int i = 0; i < enemiesDetected; i++)
            {
                if (enemyStorage[i].transform == enemy)
                {
                    stillInRange = true;
                    break;
                }
            }

            return !stillInRange;
        });

        //esto añade a los enemigos detectados al hashSet
        for(int i = 0; i < enemiesDetected; i++)
        {
            enemiesSet.Add(enemyStorage[i].transform);
        }
    }
    //detectar enemigos en una area cuadrada, usarlo como deteccion delantera del player
    public static List<Transform> DetectEnemiesInBox(Vector3 center, Vector3 halfExtents, LayerMask enemyLayer, Quaternion orientation, int maxEnemies = 15)
    {
        Collider[] enemyStorage = new Collider[maxEnemies];

        int count = Physics.OverlapBoxNonAlloc(center, halfExtents, enemyStorage, orientation, enemyLayer);

        List<Transform> result = new List<Transform>();

        for (int i = 0; i < count; i++)
        {
            result.Add(enemyStorage[i].transform);
        }

        return result;
    }

    //de esos enemigos detectados, obtener el enemigo mas cercano al player
    public static Transform FindClosestEnemy(HashSet<Transform> enemiesDetected, Transform player)
    {
        Transform closestEnemy = null;

        float closestDistanceSqr = Mathf.Infinity;

        foreach(Transform enemy in enemiesDetected)
        {
            Vector3 directionToEnemy = enemy.position - player.position;

            float sqrDistance = directionToEnemy.sqrMagnitude;

            if(sqrDistance < closestDistanceSqr)
            {
                closestDistanceSqr = sqrDistance;
                closestEnemy = enemy;
            }

        }
         
        return closestEnemy;
    }

    //version de FindClosestEnemy pero con un angulo de vision

    public static Transform FindClosestEnemyInVisionAngle(HashSet<Transform> enemiesDetected, Transform player, float visionAngle)
    {

        Transform closestEnemy = null;
        float closestDistanceSqr = Mathf.Infinity;

        foreach(Transform enemy in enemiesDetected)
        {
            Vector3 directionToEnemy = enemy.position - player.position;
            float angle = Vector3.Angle(player.forward, directionToEnemy.normalized);

            if(angle > visionAngle)
            {
                Debug.Log("the angle is above visionAngle");
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

    //de esos enemigos detectados, obtener el enemigo mas centrado a la camara
}
